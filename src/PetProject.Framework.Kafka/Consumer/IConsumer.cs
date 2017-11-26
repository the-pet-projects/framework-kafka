namespace PetProject.Framework.Kafka.Consumer
{
    using System;

    public interface IConsumer<TBaseMessage> : IDisposable
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
    }
}