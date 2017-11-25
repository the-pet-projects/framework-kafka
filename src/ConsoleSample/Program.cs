namespace ConsoleSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PetProject.Framework.Kafka.Configurations;
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
            var producerConfiguration = new ProducerConfiguration("test-client", new List<string> { "localhost:9092" });

            var producer = new Producer<TestTopic<SimpleMessage>>(producerConfiguration);

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

        internal class TestTopic<TMessage> : ITopicContract
        {
            public string TopicFullName => this.SetTopicName().TopicFullName;

            public TopicBuilder SetTopicName()
            {
                return new TopicBuilder($"{typeof(TMessage).FullName}", MessageType.Events)
                    .WithApplication("console-app")
                    .WithVersion(1);
            }
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
}