namespace Contracts
{
    using PetProject.Framework.Kafka.Topics;

    public class ItemCommandsTopic : ITopic<ItemCommandsV1>
    {
        public ItemCommandsTopic()
        {
        }

        public string TopicFullName => this.SetTopicName().TopicFullName;

        public TopicBuilder SetTopicName()
        {
            return new TopicBuilder($"{typeof(ItemCommandsV1).FullName}", MessageType.Events)
                .WithApplication("console-app")
                .WithVersion(1);
        }
    }
}