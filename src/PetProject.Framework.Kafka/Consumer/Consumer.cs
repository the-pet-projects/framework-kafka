namespace PetProject.Framework.Kafka.Consumer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configurations.Consumer;
    using Confluent.Kafka;
    using Serializer;
    using Topics;

    public abstract class Consumer<TBaseMessage> : IConsumer<TBaseMessage>
        where TBaseMessage : IMessageContract
    {
        private static readonly TimeSpan PollTimeout = TimeSpan.FromSeconds(2);

        private readonly IConsumerConfiguration configuration;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly object lockObj = new object();

        private Task task;

        protected Consumer(IConsumerConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Method to initiate the consumer. It will create a task in the background.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            lock (this.lockObj)
            {
                if (this.task != null)
                {
                    return false;
                }

                // because Poll is synchronous it will block the thread that runs this task
                // so it's better to create a task with LongRunning flag to let the task scheduler know
                // that it can create a new dedicated thread for this task instead of allocating one from the default threadpool
                this.task = Task.Factory.StartNew(
                    () =>
                    {
                        using (var consumer = new Consumer<string, TBaseMessage>(this.configuration.GetConfigurations(), new JsonDeserializer<string>(), new JsonDeserializer<TBaseMessage>()))
                        {
                            consumer.Subscribe(this.configuration.Topic.TopicFullName);
                            consumer.OnMessage += this.HandleMessage;
                            while (this.tokenSource != null && !this.tokenSource.IsCancellationRequested)
                            {
                                consumer.Poll(this.configuration.PollTimeout ?? 100);
                            }
                        }
                    },
                    TaskCreationOptions.LongRunning);

                return true;
            }
        }

        /// <summary>
        /// Must be implemented by the client in order to consumer messages and add custom treatment.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="message">Message envelope with the content to consume.</param>
        public abstract void HandleMessage(object sender, Message<string, TBaseMessage> message);

        public void Dispose()
        {
            lock (this.lockObj)
            {
                if (this.task == null)
                {
                    this.task = Task.CompletedTask;
                }
            }

            this.tokenSource.Cancel();
            this.task.Wait();
            this.tokenSource?.Dispose();
        }
    }
}