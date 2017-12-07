namespace PetProjects.Framework.Kafka.Configurations.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using PetProjects.Framework.Kafka.Exceptions;

    public class ConsumerConfiguration : IConsumerConfiguration
    {
        public ConsumerConfiguration(string groupId, string clientId, IList<string> bootstrapServers)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentException("GroupId cannot be null or whitespace. Please fix.");
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("ClientId cannot be null or whitespace. Please fix.");
            }

            if (bootstrapServers == null || !bootstrapServers.Any())
            {
                throw new ArgumentException("There is no bootstrap server configured. Please add at least one.");
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
                throw new ConsumerConfigurationException($"Interval must be greater than 0.");
            }

            this.AutoCommit = !this.AutoCommit;
            this.AutoCommitInterval = interval;

            return this;
        }

        public ConsumerConfiguration SetPollIntervalInMs(int pollInterval = 1000)
        {
            if (pollInterval <= 0)
            {
                throw new ConsumerConfigurationException($"PollInterval must be greater than 0.");
            }

            this.MaxPollIntervalInMs = pollInterval;

            return this;
        }
    }
}