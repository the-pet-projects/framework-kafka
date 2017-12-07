namespace PetProjects.Framework.Kafka.Exceptions
{
    using System;

    public class ProducerConfigurationException : Exception
    {
        public ProducerConfigurationException(string message)
            : base(message)
        {
        }
    }
}