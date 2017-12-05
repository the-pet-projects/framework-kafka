namespace PetProjects.Framework.Kafka.Producer
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Configurations.Producer;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using Contracts.Topics;
    using Exceptions;
    using Serializer;
    using Wrapper;

    public class Producer<TBaseMessage> : IProducer<TBaseMessage>
        where TBaseMessage : IMessage
    {
        private readonly Producer<string, MessageWrapper> confluentProducer;

        private readonly ITopic<TBaseMessage> topic;

        private bool disposed;

        public Producer(ITopic<TBaseMessage> topic, ProducerConfiguration configuration)
        {
            this.confluentProducer = new Producer<string, MessageWrapper>(configuration.GetConfigurations(), new StringSerializer(Encoding.UTF8), new JsonSerializer<MessageWrapper>());

            this.topic = topic;
        }

        public async Task Produce<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            var topicName = this.topic.TopicFullName;
            var partitionKey = message.GetPartitionKey();

            var wrappedMessage = MessageWrapperFactory.Create(message);

            var report = await this.confluentProducer.ProduceAsync(topicName, partitionKey, wrappedMessage);

            if (report.Error.HasError)
            {
                throw new ProducerErrorException<TMessage>(topicName, message, report.Timestamp, report.Error);
            }
        }

        public async Task<Message<string, MessageWrapper>> ProduceAsync<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            var topicName = this.topic.TopicFullName;
            var partitionKey = message.GetPartitionKey();

            var wrappedMessage = MessageWrapperFactory.Create(message);

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