namespace PetProject.Framework.Kafka.Contracts.Topics
{
    public interface ITopic<TMessage>
    {
        string TopicFullName { get; }

        TopicBuilder SetTopicName();
    }
}