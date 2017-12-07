namespace PetProjects.Framework.Kafka.Consumer
{
    using System;
    using System.Threading.Tasks;

    using Confluent.Kafka;

    using PetProjects.Framework.Kafka.Contracts.Topics;

    public interface IConsumer<TBaseMessage> : IDisposable
        where TBaseMessage : IMessage
    {
        /// <summary>
        /// Flag which signals if the consumer is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Method to initiate the consumer.
        /// Messages must be committed manually.
        /// </summary>
        void StartConsuming();

        /// <summary>
        /// Method to signal the consumer to stop.
        /// </summary>
        void StopConsuming();

        /// <summary>
        /// Decorator around Confluent Consumer to commit messages asynchronously after success.
        /// </summary>
        Task<CommittedOffsets> CommitAsync();

        /// <summary>
        /// Handler for each message to be consumed inside the topic.
        /// </summary>
        void Receive<TMessage>(Action<TMessage> handler);
    }
}