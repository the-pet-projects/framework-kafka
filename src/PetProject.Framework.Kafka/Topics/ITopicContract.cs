namespace PetProject.Framework.Kafka.Topics
{
    public interface ITopicContract
    {
        TopicBuilder SetTopicName();

        string TopicFullName { get; }
    }
}