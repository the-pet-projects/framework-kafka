namespace ConsoleProducer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts;
    using PetProject.Framework.Kafka.Configurations.Producer;
    using PetProject.Framework.Kafka.Producer;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Producer");
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var producerConfiguration = new ProducerConfiguration("test-client", new List<string> { "marx-petprojects.westeurope.cloudapp.azure.com:9092" });

            var producer = new KafkaProducer<ItemCommandsV1>(new ItemCommandsTopic(), producerConfiguration);

            for (var i = 0; i < 1; i++)
            {
                await producer.ProduceAsync(new CreateItemV1 { Type = "Event", ItemName = $"Item-{i}", Derived = true });
            }
        }
    }
}