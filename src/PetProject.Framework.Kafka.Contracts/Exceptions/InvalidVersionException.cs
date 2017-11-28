namespace PetProject.Framework.Kafka.Contracts.Exceptions
{
    using System;

    public class InvalidVersionException : Exception
    {
        public InvalidVersionException()
            : base("Version cannot be 0 or Negative. Please fix!")
        {
        }
    }
}