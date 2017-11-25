namespace PetProject.Framework.Kafka.Producer
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Configurations;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using Exceptions;
    using Newtonsoft.Json;
    using Topics;

    public class Producer<TTopic> : IProducer<TTopic>
    {
        private readonly Producer<string, string> producer;

        private readonly ITopicContract topic;

        private bool disposed;

        public Producer(ProducerConfiguration configuration)
        {
            this.producer = new Producer<string, string>(configuration.GetConfigurations(), new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));

            this.topic = Activator.CreateInstance<TTopic>() as ITopicContract;
        }

        public async Task Produce<TMessage>(TMessage message)
            where TMessage : IMessageContract
        {
            var topicName = this.topic.TopicFullName;
            var partitionKey = message.GetPartitionKey();
            var report = await this.producer.ProduceAsync(topicName, partitionKey, JsonConvert.SerializeObject(message));

            if (report.Error.HasError)
            {
                throw new ProducerErrorException<TMessage>(topicName, message, report.Timestamp, report.Error);
            }
        }

        public async Task<Message<string, string>> ProduceAsync<TMessage>(TMessage message)
            where TMessage : IMessageContract
        {
            var topicName = this.topic.TopicFullName;
            var partitionKey = message.GetPartitionKey();

            var deliveryReport = await this.producer.ProduceAsync(topicName, partitionKey, JsonConvert.SerializeObject(message));

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