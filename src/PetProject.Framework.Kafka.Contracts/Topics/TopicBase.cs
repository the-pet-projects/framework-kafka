namespace PetProjects.Framework.Kafka.Contracts.Topics
{
    public abstract class TopicBase<TMessage> : ITopic<TMessage>
    {
        protected TopicBase(string environment)
        {
            this.Environment = environment;
        }

        public string Environment { get; set; }

        public abstract TopicBuilder SetTopicName();

        public string GetTopicName() => this.SetTopicName().TopicFullName;
    }
}