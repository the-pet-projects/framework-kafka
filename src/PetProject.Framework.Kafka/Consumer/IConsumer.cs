namespace PetProject.Framework.Kafka.Consumer
{
    using System;
    using Contracts.Topics;

    public interface IConsumer<TBaseMessage> : IDisposable
        where TBaseMessage : IMessage
    {
        /// <summary>
        /// Method to initiate the consumer.
        /// Messages must be committed manually.
        /// </summary>
        bool StartConsuming();

        /// <summary>
        /// Decorator around Confluent Consumer to commit messages asynchronously after success.
        /// </summary>
        void CommitAsync();

        /// <summary>
        /// Handler for each message to be consumed inside the topic.
        /// </summary>
        void ConsumerHandlerFor<TMessage>(Action<TMessage> handler);
    }
}