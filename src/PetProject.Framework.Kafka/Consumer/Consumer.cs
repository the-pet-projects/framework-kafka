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

        private readonly Consumer<string, TBaseMessage> confluentConsumer;

        private readonly object lockObj = new object();

        private Task task;

        protected Consumer(IConsumerConfiguration configuration)
        {
            this.configuration = configuration;
            this.confluentConsumer = new Consumer<string, TBaseMessage>(this.configuration.GetConfigurations(), new JsonDeserializer<string>(), new JsonDeserializer<TBaseMessage>());
        }

        /// <inheritdoc />
        /// <summary>
        /// Method to initiate the consumer.
        /// Messages must be committed manually.
        /// Inititates a task in the backgroung with the LongRunning flag.
        /// </summary>
        public bool StartConsuming()
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
                        this.confluentConsumer.Subscribe(this.configuration.Topic.TopicFullName);

                        this.confluentConsumer.OnMessage += this.HandleMessage;

                        this.confluentConsumer.OnConsumeError += this.HandleOnConsumerError;

                        this.confluentConsumer.OnError += this.HandleError;

                        this.confluentConsumer.OnLog += this.HandleLogs;

                        this.confluentConsumer.OnStatistics += this.HandleStatistics;

                        while (this.tokenSource != null && !this.tokenSource.IsCancellationRequested)
                        {
                            this.confluentConsumer.Poll(this.configuration.PollTimeout ?? 100);
                        }
                    },
                    TaskCreationOptions.LongRunning);

                return true;
            }
        }

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

        /// <inheritdoc />
        /// <summary>
        /// Decorator around Confluent Consumer to commit messages asynchronously after success.
        /// </summary>
        public void CommitAsync()
        {
            this.confluentConsumer.CommitAsync();
        }

        /// <summary>
        /// Method to add custom treatment to Consumer Statistics.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="statistics">Statistics returned from Confluent Consumer.</param>
        protected abstract void HandleStatistics(object sender, string statistics);

        /// <summary>
        /// Method to add custom treatment to Consumer Logs.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="logMessage">Log Message object returned from Confluent Consumer.</param>
        protected abstract void HandleLogs(object sender, LogMessage logMessage);

        /// <summary>
        /// Method to add custom treatment to Consumer Errors.
        /// </summary>
        /// <param name="sender">Message sender</param>
        /// <param name="error">Error object returned from Confluent Consumer</param>
        protected abstract void HandleError(object sender, Error error);

        /// <summary>
        /// Must be implemented by the client in order to add Errors while consuming.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="message">Message envelope with the content to consume.</param>
        protected abstract void HandleOnConsumerError(object sender, Message message);

        /// <summary>
        /// Must be implemented by the client in order to consumer messages and add custom treatment.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="message">Message envelope with the content to consume.</param>
        protected abstract void HandleMessage(object sender, Message<string, TBaseMessage> message);
    }
}