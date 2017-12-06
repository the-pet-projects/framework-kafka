namespace Integration.Consumer
{
    using Contracts;
    using Microsoft.Extensions.Logging.Abstractions;
    using PetProjects.Framework.Kafka.Configurations.Consumer;
    using PetProjects.Framework.Kafka.Consumer;

    public class TestConsumer : Consumer<ItemCommandsV1>
    {
        public TestConsumer(IConsumerConfiguration configuration)
            : base(new ItemCommandsTopic(), configuration, NullLogger.Instance)
        {
        }
    }
}