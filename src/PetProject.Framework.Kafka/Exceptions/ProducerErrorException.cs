namespace PetProject.Framework.Kafka.Exceptions
{
    using System;
    using Confluent.Kafka;

    public class ProducerErrorException<TMessage> : Exception
    {
        public ProducerErrorException(Error error)
            : base("Producer Error. Check errors for more details.")
        {
            this.Error = error;
        }

        public ProducerErrorException(string topic, TMessage message, Error error)
            : this(error)
        {
            this.Topic = topic;
            this.Message = message;
        }

        public ProducerErrorException(string topic, TMessage message, Timestamp timestamp, Error error)
            : this(error)
        {
            this.Topic = topic;
            this.Message = message;
            this.Timestamp = timestamp;
        }

        public Error Error { get; }

        public string Topic { get; }

        public new TMessage Message { get; }

        public Timestamp Timestamp { get; }
    }
}