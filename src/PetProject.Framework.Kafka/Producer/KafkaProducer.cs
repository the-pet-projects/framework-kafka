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

    public class KafkaProducer<TBaseMessage> : IKafkaProducer<TBaseMessage>
        where TBaseMessage : IMessage
    {
        private readonly Producer<string, MessageWrapper<TBaseMessage>> confluentProducer;

        private readonly ITopic<TBaseMessage> topic;

        private bool disposed;

        public KafkaProducer(ITopic<TBaseMessage> topic, ProducerConfiguration configuration)
        {
            this.confluentProducer = new Producer<string, MessageWrapper<TBaseMessage>>(configuration.GetConfigurations(), new StringSerializer(Encoding.UTF8), new JsonSerializer<MessageWrapper<TBaseMessage>>());

            this.topic = topic;
        }

        public async Task Produce<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            var topicName = this.topic.TopicFullName;
            var partitionKey = message.GetPartitionKey();

            var wrappedMessage = new MessageWrapper<TBaseMessage>
            {
                MessageType = typeof(TMessage).FullName,
                Message = message
            };

            var report = await this.confluentProducer.ProduceAsync(topicName, partitionKey, wrappedMessage);

            if (report.Error.HasError)
            {
                throw new ProducerErrorException<TMessage>(topicName, message, report.Timestamp, report.Error);
            }
        }

        public async Task<Message<string, MessageWrapper<TBaseMessage>>> ProduceAsync<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            var topicName = this.topic.TopicFullName;
            var partitionKey = message.GetPartitionKey();

            var wrappedMessage = new MessageWrapper<TBaseMessage>
            {
                MessageType = typeof(TMessage).AssemblyQualifiedName,
                Message = message
            };

            var deliveryReport = await this.confluentProducer.ProduceAsync(topicName, partitionKey, wrappedMessage);

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