namespace ConsoleProducer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using PetProjects.Framework.Kafka.Configurations.Producer;
    using PetProjects.Framework.Kafka.Producer;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Producer");
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var servicesCollection = new ServiceCollection();

            var producerConfiguration = new ProducerConfiguration("test-client", new List<string> { "localhost:9092" });
            servicesCollection.AddSingleton<IProducer<ItemCommandsV1>>(new Producer<ItemCommandsV1>(new ItemCommandsTopic(), producerConfiguration));

            var serviceProvider = servicesCollection.BuildServiceProvider();

            var producer = serviceProvider.GetService<IProducer<ItemCommandsV1>>();

            for (var i = 0; i < 1; i++)
            {
                await producer.ProduceAsync(new CreateItemV1 { Type = "Command", ItemName = $"Item-{i}", Derived = true });
            }
        }
    }
}