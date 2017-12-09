namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    using System;

    using PetProjects.Framework.Kafka.Contracts.Exceptions;
    using PetProjects.Framework.Kafka.Contracts.Topics.Properties;

    public sealed class TopicBuilder
    {
        public TopicBuilder(IEntity entityName, MessageType messageType, string environment)
        {
            if (string.IsNullOrWhiteSpace(entityName.GetName()))
            {
                throw new ArgumentException("Entity Name cannot be Null or Whitespace. Please fix.");
            }

            if (string.IsNullOrWhiteSpace(environment))
            {
                throw new ArgumentException("Environment cannot be Null or Whitespace. Please fix.");
            }

            this.EntityName = entityName.GetName().ToLowerInvariant();
            this.MessageType = messageType.ToString().ToLowerInvariant();
            this.Environment = environment.ToLowerInvariant();
        }

        public string EntityName { get; }

        public string Environment { get; }

        public string MessageType { get; }

        public string ApplicationName { get; private set; }

        public int? Version { get; private set; }

        public string TopicFullName => this.Build();

        public TopicBuilder WithApplication(IApplication applicationName)
        {
            if (string.IsNullOrWhiteSpace(applicationName.GetName()))
            {
                throw new InvalidApplicationNameException();
            }

            this.ApplicationName = applicationName.GetName().ToLowerInvariant();

            return this;
        }

        public TopicBuilder WithVersion(int version)
        {
            if (version <= 0)
            {
                throw new InvalidVersionException();
            }

            this.Version = version;

            return this;
        }

        private string Build()
        {
            var topicName = $"{this.Environment}.{this.EntityName}.{this.MessageType}";

            if (!string.IsNullOrWhiteSpace(this.ApplicationName))
            {
                topicName = $"{this.ApplicationName}.{topicName}";
            }

            if (this.Version != null)
            {
                topicName = $"{topicName}-v{this.Version}";
            }

            return topicName;
        }
    }
}