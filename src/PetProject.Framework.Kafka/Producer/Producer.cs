namespace PetProjects.Framework.Kafka.Producer
{
    using System.Text;
    using System.Threading.Tasks;

    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;

    using PetProjects.Framework.Kafka.Configurations.Producer;
    using PetProjects.Framework.Kafka.Contracts.Topics;
    using PetProjects.Framework.Kafka.Serializer;
    using PetProjects.Framework.Kafka.Wrapper;

    public class Producer<TBaseMessage> : IProducer<TBaseMessage>
        where TBaseMessage : IMessage
    {
        private readonly Producer<string, MessageWrapper> confluentProducer;

        private readonly ITopic<TBaseMessage> topic;

        public Producer(ITopic<TBaseMessage> topic, IProducerConfiguration configuration)
        {
            this.confluentProducer = new Producer<string, MessageWrapper>(configuration.GetConfigurations(), new StringSerializer(Encoding.UTF8), new JsonSerializer<MessageWrapper>());

            this.topic = topic;
        }

        public void Produce<TMessage>(TMessage message, IDeliveryHandler<string, MessageWrapper> deliveryHandler = null)
            where TMessage : IMessage
        {
            var partitionKey = message.GetPartitionKey();

            var wrappedMessage = MessageWrapperFactory.Create(message);

            this.confluentProducer.ProduceAsync(this.topic.Name, partitionKey, wrappedMessage, deliveryHandler);
        }

        public async Task<Message<string, MessageWrapper>> ProduceAsync<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            var partitionKey = message.GetPartitionKey();

            var wrappedMessage = MessageWrapperFactory.Create(message);

            var deliveryReport = await this.confluentProducer.ProduceAsync(this.topic.Name, partitionKey, wrappedMessage).ConfigureAwait(false);

            return deliveryReport;
        }

        public void Dispose()
        {
            this.confluentProducer?.Dispose();
        }
    }
}