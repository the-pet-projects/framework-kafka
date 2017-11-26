namespace PetProject.Framework.Kafka.Consumer
{
    using System;
    using Confluent.Kafka;

    public interface IConsumer<TBaseMessage> : IDisposable
    {
        bool Start();

        void HandleMessage(object sender, Message<string, TBaseMessage> message);
    }
}