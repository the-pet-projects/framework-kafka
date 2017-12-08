namespace Integration.Consumer
{
    using System;

    using System.Linq;
    using System.Threading;

    using Integration.Consumer.Configs;
    using Integration.Contracts;

    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using PetProjects.Framework.Kafka.Consumer;

    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            Console.WriteLine("Consumer");

            var serviceProvider = new Configurations().ServiceProvider;

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

            Console.WriteLine("Ctrl-C to exit.");

            Console.CancelKeyPress += (_, e) =>
            {
                Program.QuitEvent.Set();
                e.Cancel = true; // prevent the process from terminating.
            };

            Program.QuitEvent.WaitOne();

            consumer.Dispose();
            Console.WriteLine("Terminating consumer.");
        }

        private static void HandleCreateItem(CreateItemV1 message)
        {
            Console.WriteLine($"Message: {JsonConvert.SerializeObject(message)} |  Partition: {message.GetPartitionKey()} | Derived: {message.Derived}");
        }
    }
}