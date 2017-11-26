namespace Contracts
{
    using Confluent.Kafka;
    using PetProject.Framework.Kafka.Topics;

    public class ItemCommandsV1 : IMessage
    {
        public ItemCommandsV1()
        {
            this.Timestamp = new Timestamp();
        }

        public string Type { get; set; }

        public string ItemName { get; set; }

        public Timestamp Timestamp { get; set; }

        public string GetPartitionKey()
        {
            return $"{this.Type}";
        }
    }
}