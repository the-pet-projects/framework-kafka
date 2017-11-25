namespace PetProject.Framework.Kafka.Topics
{
    using System;

    public interface IMessageContract
    {
        Type MessageType { get; }

        string GetPartitionKey();
    }
}