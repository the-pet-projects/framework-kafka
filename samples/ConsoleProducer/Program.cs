namespace ConsoleProducer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts;
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
            var producerConfiguration = new ProducerConfiguration("test-client", new List<string> { "localhost:9092" });

            var producer = new Producer<ItemCommandsV1>(new ItemCommandsTopic(), producerConfiguration);

            for (var i = 0; i < 1; i++)
            {
                await producer.ProduceAsync(new CreateItemV1 { Type = "Event", ItemName = $"Item-{i}", Derived = true });
            }
        }
    }
}