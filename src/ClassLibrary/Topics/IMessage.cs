namespace PetProject.Framework.Kafka.Contracts.Topics
{
    public interface IMessage
    {
        string GetPartitionKey();
    }
}