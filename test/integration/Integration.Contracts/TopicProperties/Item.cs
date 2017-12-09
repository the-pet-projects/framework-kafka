namespace Integration.Contracts.TopicProperties
{
    using PetProjects.Framework.Kafka.Contracts.Topics.Properties;

    public class Item : IEntity
    {
        public Item()
        {
            this.Name = $"{nameof(Item)}";
        }

        public string Name { get; }

        public string GetName() => this.Name;
    }
}