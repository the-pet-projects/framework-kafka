namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    public abstract class TopicBase<TMessage> : ITopic<TMessage>
    {
        private readonly TopicConfig config;

        protected TopicBase(TopicConfig config)
        {
            this.config = config;
        }

        public string Name => this.SetupTopicBuilder().TopicFullName;

        public TopicBuilder SetupTopicBuilder()
        {
            return new TopicBuilder(this.config);
        }
    }
}