namespace PetProject.Framework.Kafka.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using Newtonsoft.Json;
    using Topics;

    public class Producer<TTopic> : IProducer, IDisposable
        where TTopic : ITopicContract
    {
        private Dictionary<string, object> Configuration = new Dictionary<string, object>();

        private Producer<string, string> producer;

        private ITopicContract Topic;

        private bool disposed = false;

        public Producer(string bootstrapServers)
        {
            if (string.IsNullOrWhiteSpace(bootstrapServers))
            {
                throw new ArgumentException(nameof(bootstrapServers));
            }

            this.Configuration.Add("bootstrap.servers", bootstrapServers);

            this.producer = new Producer<string, string>(this.Configuration, new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));

            this.Topic = Activator.CreateInstance<TTopic>();
        }

        public async Task ProduceAsync<TMessage>(TMessage message)
            where TMessage : IMessageContract
        {
            await this.producer.ProduceAsync(this.Topic.GetTopicName(), message.GetPartitionKey(), JsonConvert.SerializeObject(message));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Dispose();
            }

            disposed = true;
        }
    }
}