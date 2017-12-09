namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    public interface ITopic<TMessage>
    {
        TopicBuilder SetTopicName();

        string GetTopicName();
    }
}