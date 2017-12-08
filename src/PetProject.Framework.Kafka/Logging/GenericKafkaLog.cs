namespace PetProjects.Framework.Kafka.Logging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Wrapper to segregate Kafka logs in kibana.
    /// </summary>
    public class GenericKafkaLog
    {
        private readonly ILogger logger;

        public GenericKafkaLog(ILogger logger)
        {
            this.logger = logger;
        }

        public void KafkaLogWarning(string message, params object[] args)
        {
            this.logger.KafkaLogWarning(message, args);
        }

        public void KafkaLogInfo(string message, params object[] args)
        {
            this.logger.KafkaLogInfo(message, args);
        }

        public void KafkaLogCritical(string message, params object[] args)
        {
            this.logger.KafkaLogCritical(message, args);
        }

        public void KafkaLogError(string message, params object[] args)
        {
            this.logger.KafkaLogError(message, args);
        }

        public void KafkaLogTrace(string message, params object[] args)
        {
            this.logger.KafkaLogTrace(message, args);
        }
    }
}