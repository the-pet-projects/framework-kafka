namespace Integration.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Integration.Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using PetProjects.Framework.Kafka.Configurations.Consumer;
    using PetProjects.Framework.Kafka.Consumer;

    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Consumer");

            var servicesCollection = new ServiceCollection();

            servicesCollection.AddSingleton<IConsumerConfiguration>(
                new ConsumerConfiguration(
                        "group01",
                        "consumer01",
                        new List<string>
                        {
                            "marx-petprojects.westeurope.cloudapp.azure.com:9092"
                        })
                    .SetPollIntervalInMs(10000));

            servicesCollection.AddSingleton<IConsumer<ItemCommandsV1>, TestConsumer>();

            var serviceProvider = servicesCollection.BuildServiceProvider();

            var consumer = serviceProvider.GetService<IConsumer<ItemCommandsV1>>();

            consumer.Receive<CreateItemV1>((message) =>
            {
                HandleCreateItem(message);
                var committedOffsets = consumer.CommitAsync().Result;

                if (committedOffsets.Offsets.Any())
                {
                    Console.WriteLine($"CommittedOffsets: {JsonConvert.SerializeObject(committedOffsets)}");
                }
            });

            consumer.StartConsuming();

            Console.WriteLine(consumer.IsRunning ? "Started!!" : "Not Started!!");

            var cancelled = false;
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cancelled = true;
            };

            Console.WriteLine("Ctrl-C to exit.");
            while (!cancelled)
            {
                consumer.StopConsuming();
            }
        }

        private static void HandleCreateItem(CreateItemV1 message)
        {
            Console.WriteLine($"Message: {JsonConvert.SerializeObject(message)} |  Partition: {message.GetPartitionKey()} | Derived: {message.Derived}");
        }
    }
}