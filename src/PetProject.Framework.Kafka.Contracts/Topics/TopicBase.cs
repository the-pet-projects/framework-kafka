namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    public abstract class TopicBase<TMessage> : ITopic<TMessage>
    {
        public string Name => this.SetupTopicBuilder().TopicFullName;

        public abstract TopicBuilder SetupTopicBuilder();
    }
}