namespace ConsoleConsumer
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using PetProject.Framework.Kafka.Configurations.Consumer;
    using PetProject.Framework.Kafka.Consumer;

    internal partial class Program
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
                    }
                    ).SetPollTimeout(10000));

            servicesCollection.AddSingleton<IConsumer<ItemCommandsV1>, TestConsumer>();

            var serviceProvider = servicesCollection.BuildServiceProvider();

            var consumer = serviceProvider.GetService<IConsumer<ItemCommandsV1>>();

            consumer.ConsumerHandlerFor<ItemCommandsV1>((message) =>
            {
                HandleGenericItemCommands(message);
                CommitAsync(consumer);
            });

            consumer.ConsumerHandlerFor<CreateItemV1>((message) =>
            {
                HandleCreateItem(message);
                CommitAsync(consumer);
            });

            var initiated = consumer.StartConsuming();

            Console.WriteLine(initiated ? "Started!!" : "Not Started!!");

            var cancelled = false;
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cancelled = true;
            };

            Console.WriteLine("Ctrl-C to exit.");
            while (!cancelled)
            {
                consumer.Dispose();
            }
        }

        private static void HandleGenericItemCommands(ItemCommandsV1 message)
        {
            Console.WriteLine($"Message: {JsonConvert.SerializeObject(message)} |  Partition: {message.GetPartitionKey()}");
        }

        private static void HandleCreateItem(CreateItemV1 message)
        {
            Console.WriteLine($"Message: {JsonConvert.SerializeObject(message)} |  Partition: {message.GetPartitionKey()} | Derived: {message.Derived}");
        }

        private static void CommitAsync(IConsumer<ItemCommandsV1> consumer)
        {
            consumer.CommitAsync();
        }
    }
}