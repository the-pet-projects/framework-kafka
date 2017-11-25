namespace PetProject.Framework.Kafka.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using Exceptions;
    using Newtonsoft.Json;
    using Topics;

    public class Producer<TTopic> : IProducer<TTopic>
    {
        private readonly Dictionary<string, object> configuration = new Dictionary<string, object>();

        private readonly Producer<string, string> producer;

        private readonly ITopicContract topic;

        private bool disposed;

        public Producer(string bootstrapServers)
        {
            if (string.IsNullOrWhiteSpace(bootstrapServers))
            {
                throw new ArgumentException(nameof(bootstrapServers));
            }

            this.configuration.Add("bootstrap.servers", bootstrapServers);

            this.producer = new Producer<string, string>(this.configuration, new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));

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