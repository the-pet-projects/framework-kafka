namespace Integration.Contracts
{
    using PetProjects.Framework.Kafka.Contracts.Topics;
    using TopicProperties;

    public class ItemCommandsTopic : TopicBase<ItemCommandsV1>
    {
        public ItemCommandsTopic(string environment)
            : base(environment)
        {
        }

        public override TopicBuilder SetTopicName()
        {
            return new TopicBuilder(new Item(), MessageType.Commands, this.Environment)
                        .WithApplication(new TestApp())
                        .WithVersion(1);
        }
    }
}