namespace PetProject.Framework.Kafka.Exceptions
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