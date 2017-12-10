namespace Integration.Consumer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configs;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using PetProjects.Framework.Kafka.Consumer;

    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            Console.WriteLine("Consumer");

            var serviceProvider = new Configurations().ServiceProvider;

            var consumer = serviceProvider.GetService<IConsumer<ItemCommandsV1>>();
            
            consumer.TryReceiveAsync<CreateItemV1>(async (command) => await Program.HandleCreateItemAsync(command));

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

        private static Task<bool> HandleCreateItemAsync(CreateItemV1 command)
        {
            Console.WriteLine($"Message: {JsonConvert.SerializeObject(command)} |  Partition: {command.GetPartitionKey()} | Derived: {command.Derived}");

            return Task.FromResult(command != null);
        }

        private static void HandleCreateItem(CreateItemV1 message)
        {
            Console.WriteLine($"Message: {JsonConvert.SerializeObject(message)} |  Partition: {message.GetPartitionKey()} | Derived: {message.Derived}");
        }
    }
}