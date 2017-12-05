using PetProjects.Framework.Kafka.Contracts.Topics;

namespace Contracts
{
    public class ItemCommandsTopic : ITopic<ItemCommandsV1>
    {
        public ItemCommandsTopic()
        {
        }

        public string TopicFullName => this.SetTopicName().TopicFullName;

        public TopicBuilder SetTopicName()
        {
            return new TopicBuilder($"{typeof(ItemCommandsV1).FullName}", MessageType.Commands)
                .WithApplication("console-app-test")
                .WithVersion(1);
        }
    }
}