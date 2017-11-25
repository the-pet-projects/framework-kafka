namespace ConsoleSample
{
    using System.Threading.Tasks;
    using PetProject.Framework.Kafka.Producer;
    using PetProject.Framework.Kafka.Topics;

    internal class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var producer = new Producer<TestTopic>("marx-petprojects.westeurope.cloudapp.azure.com:9092");

            for (var i = 0; i < 10; i++)
            {
                if ((i % 2) == 0)
                {
                    await producer.ProduceAsync(new DerivedMessage { Type = "1", Message = $"text-{i}", Derived = true });
                }
                else
                {
                    await producer.ProduceAsync(new SimpleMessage { Type = "1", Message = $"text-{i}" });
                }
            }
        }
    }

    internal class TestTopic : ITopicContract
    {
        public string GetTopicName()
        {
            return "testing";
        }
    }

    internal class SimpleMessage : IMessageContract

    {
        public string Type { get; set; }

        public string Message { get; set; }

        public string GetPartitionKey()
        {
            return $"{this.Type}";
        }
    }

    internal class DerivedMessage : SimpleMessage
    {
        public bool Derived { get; set; }
    }
}