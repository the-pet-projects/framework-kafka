namespace PetProjects.Framework.Kafka.Configurations.Producer
{
    using System.Collections.Generic;
    using System.Linq;

    using PetProjects.Framework.Kafka.Exceptions;

    public class ProducerConfiguration : IProducerConfiguration
    {
        private const int MaxInFlightRequests = 5;

        private readonly Dictionary<AcksType, string> acksMap = new Dictionary<AcksType, string>
        {
            { AcksType.None, "0" },
            { AcksType.AtLeastOne, "1" },
            { AcksType.All, "all" }
        };

        public ProducerConfiguration(string clientId, IList<string> bootstrapServers)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ProducerConfigurationException(ExceptionMessages.Common.InvalidBoostrapServers);
            }

            if (!bootstrapServers.Any())
            {
                throw new ProducerConfigurationException(ExceptionMessages.Common.InvalidBoostrapServers);
            }

            this.Configurations = new Dictionary<string, object>
            {
                { "bootstrap.servers", string.Join(",", bootstrapServers) },
                { "client.id", clientId }
            };
        }

        public ProducerConfiguration(string clientId, string bootstrapServers)
            : this(clientId, bootstrapServers.Split(',').ToList())
        {
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
                throw new ProducerConfigurationException(ExceptionMessages.ProducerErrorMessages.InvalidRetriesIdempotence);
            }

            if (maxInFlightRequestPerConnection <= 0 || maxInFlightRequestPerConnection > MaxInFlightRequests)
            {
                throw new ProducerConfigurationException(ExceptionMessages.ProducerErrorMessages.InvalidMaxInFlightRequestPerConnectionIdempotence(MaxInFlightRequests));
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
            if (batchSize <= 0)
            {
                throw new ProducerConfigurationException(ExceptionMessages.ProducerErrorMessages.InvalidBatchSizeInput);
            }

            this.BatchSize = batchSize;

            this.Configurations.Add("batch.size", batchSize);

            return this;
        }
    }
}