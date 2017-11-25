namespace PetProject.Framework.Kafka.Topics
{
    public interface IMessageContract
    {
        string GetPartitionKey();
    }
}