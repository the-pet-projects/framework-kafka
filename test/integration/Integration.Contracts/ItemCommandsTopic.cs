namespace Integration.Contracts
{
    using PetProjects.Framework.Kafka.Contracts.Topics;

    public class ItemCommandsTopic : ITopic<ItemCommandsV1>
    {
        public ItemCommandsTopic()
        {
        }

        public string TopicFullName => this.SetTopicName("ci").TopicFullName;

        public TopicBuilder SetTopicName(string environment)
        {
            return new TopicBuilder($"{typeof(ItemCommandsV1).FullName}", MessageType.Commands, environment)
                .WithApplication("console-app-test")
                .WithVersion(1);
        }
    }
}