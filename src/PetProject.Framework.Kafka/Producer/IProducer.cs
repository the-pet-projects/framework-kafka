namespace PetProject.Framework.Kafka.Producer
{
    using System;
    using System.Threading.Tasks;
    using Topics;

    internal interface IProducer<TTopic> : IDisposable
    {
        Task Produce<TMessage>(TMessage message) where TMessage : IMessageContract;
    }
}