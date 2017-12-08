namespace PetProjects.Framework.Kafka.Configurations.Consumer
{
    using System.Collections.Generic;
    using System.Linq;

    using PetProjects.Framework.Kafka.Exceptions;

    public class ConsumerConfiguration : IConsumerConfiguration
    {
        public ConsumerConfiguration(string groupId, string clientId, IList<string> bootstrapServers)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ConsumerConfigurationException(ExceptionMessages.Common.InvalidGroupIdInput);
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ConsumerConfigurationException(ExceptionMessages.Common.InvalidClientIdInput);
            }

            if (bootstrapServers == null || !bootstrapServers.Any())
            {
                throw new ConsumerConfigurationException(ExceptionMessages.Common.InvalidBoostrapServers);
            }

            this.Configurations = new Dictionary<string, object>
            {
                { "bootstrap.servers", string.Join(",", bootstrapServers) },
                { "client.id", clientId },
                { "group.id", groupId },
                { "enable.auto.commit", this.AutoCommit },
                { "auto.commit.interval.ms", this.AutoCommitInterval }
            };
        }

        public Dictionary<string, object> Configurations { get; }

        public int MaxPollIntervalInMs { get; private set; } = 5000;

        public bool AutoCommit { get; private set; }

        public int AutoCommitInterval { get; private set; } = 300000;

        public Dictionary<string, object> GetConfigurations()
        {
            return this.Configurations;
        }

        public ConsumerConfiguration EnableAutoCommit(int interval = 5000)
        {
            if (interval <= 0)
            {
                throw new ConsumerConfigurationException(ExceptionMessages.ConsumerErrorMessages.InvalidIntervalInput);
            }

            this.AutoCommit = true;
            this.AutoCommitInterval = interval;

            return this;
        }

        public ConsumerConfiguration SetPollIntervalInMs(int pollInterval = 1000)
        {
            if (pollInterval <= 0)
            {
                throw new ConsumerConfigurationException(ExceptionMessages.ConsumerErrorMessages.InvalidPollIntervalInput);
            }

            this.MaxPollIntervalInMs = pollInterval;

            return this;
        }
    }
}