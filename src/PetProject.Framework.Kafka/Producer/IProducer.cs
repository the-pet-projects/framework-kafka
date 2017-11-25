namespace PetProject.Framework.Kafka.Producer
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Topics;

    internal interface IProducer<TTopic> : IDisposable
    {
        Task Produce<TMessage>(TMessage message) where TMessage : IMessageContract;

        Task<Message<string, string>> ProduceAsync<TMessage>(TMessage message) where TMessage : IMessageContract;
    }
}