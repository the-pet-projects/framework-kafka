namespace Integration.Producer
{
    using System;
    using System.Threading.Tasks;
    using Configs;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using PetProjects.Framework.Kafka.Producer;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Producer");
            MainAsync(args).Wait();
        }

        // topic name : console-app-test.integration.contracts.itemcommandsv1.commands-v1
        private static async Task MainAsync(string[] args)
        {
            var producer = new Configurations().ServiceProvider.GetService<IProducer<ItemCommandsV1>>();

            for (var i = 0; i < 2; i++)
            {
                var report = await producer.ProduceAsync(new CreateItemV1 { Type = "Command", ItemName = $"Item-{i}", Derived = true });

                Console.WriteLine($"Report: {JsonConvert.SerializeObject(report)}");
            }

            Console.ReadLine();
        }
    }
}