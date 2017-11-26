namespace PetProject.Framework.Kafka.Topics
{
    public interface ITopic<TMessage>
    {
        string TopicFullName { get; }

        TopicBuilder SetTopicName();
    }
}