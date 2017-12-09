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
        /// Handler for each message to be consumed inside the topic.
        /// </summary>
        void Receive<TMessage>(Action<TMessage> handler);
    }
}