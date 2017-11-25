namespace PetProject.Framework.Kafka.Topics
{
    public interface ITopicContract
    {
        string TopicFullName { get; }

        TopicBuilder SetTopicName();
    }
}