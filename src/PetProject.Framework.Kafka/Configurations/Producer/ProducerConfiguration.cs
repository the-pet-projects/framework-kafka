namespace PetProject.Framework.Kafka.Configurations.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProducerConfiguration
    {
        private readonly Dictionary<AcksType, string> acksMap = new Dictionary<AcksType, string>
        {
            { AcksType.None, "0" },
            { AcksType.AtLeastOne, "1" },
            { AcksType.All, "all" }
        };

        public ProducerConfiguration(string clientId, IList<string> bootstrapServers)
        {
            if (!bootstrapServers.Any())
            {
                throw new ArgumentException("There is no bootstrap server configured. Please add at least one.");
            }

            this.Configurations = new Dictionary<string, object>
            {
                { "bootstrap.servers", string.Join(",", bootstrapServers) },
                { "client.id", clientId }
            };
        }



        protected Dictionary<string, object> Configurations { get; }

        public Dictionary<string, object> GetConfigurations()
        {
            return this.Configurations;
        }

        public ProducerConfiguration WithBatch(int batchSize)
        {
            this.Configurations.Add("batch.size", batchSize);

            return this;
        }

        public ProducerConfiguration WithIdempotence(int retries, int maxInFlightRequestPerConnection = 1)
        {
            this.Configurations.Add("acks", "all");
            this.Configurations.Add("retries", retries);
            this.Configurations.Add("max.in.flight.requests.per.connection", maxInFlightRequestPerConnection);

            return this;
        }
    }
}