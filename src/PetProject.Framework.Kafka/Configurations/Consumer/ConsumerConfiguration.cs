namespace PetProject.Framework.Kafka.Configurations.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Topics;

    public class ConsumerConfiguration : IConsumerConfiguration
    {
        public ConsumerConfiguration(string groupId, string clientId, IList<string> bootstrapServers, ITopicContract topic)
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
                { "group.id", groupId }
            };

            this.Topic = topic;
        }

        public Dictionary<string, object> Configurations { get; }

        public ITopicContract Topic { get; }

        public int? PollTimeout { get; private set; }

        public Dictionary<string, object> GetConfigurations()
        {
            return this.Configurations;
        }

        public ConsumerConfiguration SetPollTimeout(int pollTimeout)
        {
            this.PollTimeout = pollTimeout;

            return this;
        }
    }
}