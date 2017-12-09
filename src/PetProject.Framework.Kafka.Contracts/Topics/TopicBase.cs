namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    public abstract class TopicBase<TMessage> : ITopic<TMessage>
    {
        public abstract TopicBuilder SetupTopicBuilder();

        public string Name => this.SetupTopicBuilder().TopicFullName;
    }
}