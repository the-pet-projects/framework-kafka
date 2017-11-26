namespace PetProject.Framework.Kafka.Producer
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Configurations.Producer;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using Exceptions;
    using Serializer;
    using Topics;

    public class KafkaProducer<TTopic, TMessage> : IKafkaProducer<TTopic, TMessage>
        where TMessage : IMessageContract
        where TTopic : ITopicContract
    {
        private readonly Producer<string, TMessage> confluentProducer;

        private readonly ITopicContract topic;

        private bool disposed;

        public KafkaProducer(ProducerConfiguration configuration)
        {
            this.confluentProducer = new Producer<string, TMessage>(configuration.GetConfigurations(), new StringSerializer(Encoding.UTF8), new JsonSerializer<TMessage>());

            this.topic = Activator.CreateInstance<TTopic>();
        }

        public async Task Produce(TMessage message)
        {
            var topicName = this.topic.TopicFullName;
            var partitionKey = message.GetPartitionKey();
            var report = await this.confluentProducer.ProduceAsync(topicName, partitionKey, message);

            if (report.Error.HasError)
            {
                throw new ProducerErrorException<TMessage>(topicName, message, report.Timestamp, report.Error);
            }
        }

        public async Task<Message<string, TMessage>> ProduceAsync(TMessage message)
        {
            var topicName = this.topic.TopicFullName;
            var partitionKey = message.GetPartitionKey();

            var deliveryReport = await this.confluentProducer.ProduceAsync(topicName, partitionKey, message);

            return deliveryReport;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Dispose();
            }

            this.disposed = true;
        }
    }
}