namespace Integration.Contracts
{
    using PetProjects.Framework.Kafka.Contracts.Topics;

    public class ItemCommandsTopic : TopicBase<ItemCommandsV1>
    {
        public override TopicBuilder SetupTopicBuilder()
        {
            return new TopicBuilder(new TopicConfig(nameof(CreateItemV1), MessageType.Commands, "console-app-test", "dev", 1));
        }
    }
}