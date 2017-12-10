namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    public interface ITopic<TMessage>
    {
        string Name { get; }

        TopicBuilder SetupTopicBuilder();
    }
}