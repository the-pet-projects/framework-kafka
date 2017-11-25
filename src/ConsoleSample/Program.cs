namespace ConsoleSample
{
    using System;
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
            var producer = new Producer<TestTopic<SimpleMessage>>("marx-petprojects.westeurope.cloudapp.azure.com:9092");

            for (var i = 0; i < 10; i++)
            {
                if ((i % 2) == 0)
                {
                    await producer.ProduceAsync(new DerivedMessage { Type = "Event", Message = $"text-{i}", Derived = true });
                }
                else
                {
                    await producer.ProduceAsync(new SimpleMessage { Type = "None", Message = $"text-{i}" });
                }
            }
        }
    }

    internal class TestTopic<TMessage> : ITopicContract
    {
        public TopicBuilder SetTopicName()
        {
            return new TopicBuilder($"{typeof(TMessage).FullName}", MessageType.Events)
                .WithApplication("console-app")
                .WithVersion(1);
        }

        public string TopicFullName => this.SetTopicName().TopicFullName;
    }

    internal class SimpleMessage : IMessageContract
    {
        public Type MessageType => typeof(SimpleMessage);

        public string Type { get; set; }

        public string Message { get; set; }

        public string GetPartitionKey()
        {
            return $"{this.Type}";
        }
    }

    internal class DerivedMessage : SimpleMessage
    {
        public new Type MessageType => typeof(DerivedMessage);

        public bool Derived { get; set; }
    }
}