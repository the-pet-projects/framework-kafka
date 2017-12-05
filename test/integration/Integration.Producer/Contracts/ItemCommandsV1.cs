namespace Integration.TestContracts
{
    using System;
    using PetProjects.Framework.Kafka.Contracts.Topics;

    public class ItemCommandsV1 : IMessage
    {
        public ItemCommandsV1()
        {
            this.DateTime = DateTime.UtcNow;
        }

        public string Type { get; set; }

        public string ItemName { get; set; }

        public DateTime DateTime { get; set; }

        public string GetPartitionKey()
        {
            return $"{this.Type}";
        }
    }
}