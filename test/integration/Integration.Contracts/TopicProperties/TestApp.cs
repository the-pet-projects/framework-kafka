namespace Integration.Contracts.TopicProperties
{
    using PetProjects.Framework.Kafka.Contracts.Topics.Properties;

    public class TestApp : IApplication
    {
        public TestApp()
        {
            this.Name = $"{nameof(TestApp)}";
        }

        public string Name { get; }

        public string GetName() => this.Name;
    }
}