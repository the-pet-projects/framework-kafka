namespace PetProject.Framework.Kafka.Topics
{
    public interface IMessage
    {
        string GetPartitionKey();
    }
}