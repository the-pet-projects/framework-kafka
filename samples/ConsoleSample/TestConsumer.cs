namespace ConsoleConsumer
{
    using Confluent.Kafka;
    using Contracts;
    using PetProject.Framework.Kafka.Configurations.Consumer;
    using PetProject.Framework.Kafka.Consumer;

    public class TestConsumer : Consumer<ItemCommandsV1>
    {
        public TestConsumer(IConsumerConfiguration configuration)
            : base(new ItemCommandsTopic(), configuration)
        {
        }

        protected override void HandleStatistics(object sender, string statistics)
        {
        }

        protected override void HandleLogs(object sender, LogMessage logMessage)
        {
        }

        protected override void HandleError(object sender, Error error)
        {
        }

        protected override void HandleOnConsumerError(object sender, Message message)
        {
        }
    }
}