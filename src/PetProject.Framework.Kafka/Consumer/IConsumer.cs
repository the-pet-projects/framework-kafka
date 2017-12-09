namespace PetProjects.Framework.Kafka.Consumer
{
    using System;

    using PetProjects.Framework.Kafka.Contracts.Topics;

    public interface IConsumer<TBaseMessage> : IDisposable
        where TBaseMessage : IMessage
    {
        /// <summary>
        /// Method to initiate the consumer.
        /// Messages must be committed manually.
        /// </summary>
        void StartConsuming();

        /// <summary>
        /// Handler for each message to be consumed inside the topic. If handler doesn't throw exception, message will be commited.
        /// </summary>
        void Receive<TMessage>(Action<TMessage> handler);

        /// <summary>
        /// Handler for each message to be consumed inside the topic.
        /// Return type of handler should be a boolean that will be used to determine if message is to be commited or not.
        /// </summary>
        void TryReceive<TMessage>(Func<TMessage, bool> handler);
    }
}