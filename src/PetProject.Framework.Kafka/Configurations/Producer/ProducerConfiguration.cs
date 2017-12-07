namespace PetProjects.Framework.Kafka.Configurations.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using PetProjects.Framework.Kafka.Exceptions;

    public class ProducerConfiguration : IProducerConfiguration
    {
        private const int MaxInFlightRequest = 5;

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

        public ProducerConfiguration(string clientId, string bootstrapServers)
        {
            if (string.IsNullOrWhiteSpace(bootstrapServers))
            {
                throw new ArgumentException("There is no bootstrap server configured. Please add at least one.");
            }

            this.Configurations = new Dictionary<string, object>
            {
                { "bootstrap.servers", bootstrapServers },
                { "client.id", clientId }
            };
        }

        public AcksType Acks { get; private set; }

        public int Retries { get; private set; }

        public int BatchSize { get; private set; }

        public int MaxInFlightRequestPerConnection { get; private set; }

        protected Dictionary<string, object> Configurations { get; }

        public ProducerConfiguration EnableIdempotence(int maxInFlightRequestPerConnection, int retries)
        {
            if (retries <= 0)
            {
                throw new ProducerConfigurationException($"Retries must be greater than 0.");
            }

            if (maxInFlightRequestPerConnection <= 0 || maxInFlightRequestPerConnection > MaxInFlightRequest)
            {
                throw new ProducerConfigurationException($"MaxInFlightRequestPerConnection must be less than or equal to {MaxInFlightRequest}.");
            }

            this.Acks = AcksType.All;
            this.Retries = retries;
            this.MaxInFlightRequestPerConnection = maxInFlightRequestPerConnection;

            this.Configurations.Add("acks", "all");
            this.Configurations.Add("retries", retries);
            this.Configurations.Add("max.in.flight.requests.per.connection", maxInFlightRequestPerConnection);

            return this;
        }

        public Dictionary<string, object> GetConfigurations()
        {
            return this.Configurations;
        }

        public ProducerConfiguration WithBatch(int batchSize)
        {
            this.Configurations.Add("batch.size", batchSize);

            return this;
        }
    }
}