namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    public interface IMessage
    {
        string GetPartitionKey();
    }
}