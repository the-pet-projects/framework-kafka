namespace PetProjects.Framework.Kafka.Exceptions
{
    using System;

    public class ConsumerConfigurationException : Exception
    {
        public ConsumerConfigurationException(string message)
            : base(message)
        {
        }
    }
}