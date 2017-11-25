namespace PetProject.Framework.Kafka.Exceptions
{
    using System;

    public class InvalidApplicationNameException : Exception
    {
        public InvalidApplicationNameException()
            : base("Application Name cannot be Null or Whitespace. Please fix!")
        {
        }
    }
}