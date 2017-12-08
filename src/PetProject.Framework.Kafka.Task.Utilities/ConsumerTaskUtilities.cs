namespace PetProject.Framework.Kafka.Task.Utilities
{
    using System.Threading.Tasks;

    using PetProjects.Framework.Kafka.Consumer;
    using PetProjects.Framework.Kafka.Contracts.Topics;

    public static class ConsumerTaskUtilities
    {
        public static Task StartLongRunningConsumer<TBaseMessage>(IConsumer<TBaseMessage> consumer)
            where TBaseMessage : IMessage
        {
            return Task.Factory.StartNew(consumer.StartConsuming, TaskCreationOptions.LongRunning);
        }
    }
}