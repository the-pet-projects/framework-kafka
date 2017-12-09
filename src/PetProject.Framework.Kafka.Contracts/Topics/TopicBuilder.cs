namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    public sealed class TopicBuilder
    {
        private readonly TopicConfig config;

        public TopicBuilder(TopicConfig config)
        {
            this.config = config;
        }

        public string TopicFullName => this.Build();

        private string Build()
        {
            return $"{this.config.Environment}.{this.config.Application}-{this.config.EntityName}_{this.config.MessageType}.{this.config.Version}";
        }
    }
}