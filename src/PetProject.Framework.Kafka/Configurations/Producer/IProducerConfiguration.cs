namespace PetProject.Framework.Kafka.Configurations.Producer
{
    using System.Collections.Generic;

    //// TODO: Move comments to variables in ProducerConfiguration
    public interface IProducerConfiguration
    {
        /// <summary>
        ///  A list of host/port pairs to use for establishing the initial connection to the Kafka cluster.
        /// The client will make use of all servers irrespective of which servers are specified here for
        /// bootstrapping—this list only impacts the initial hosts used to discover the full set of servers.
        /// </summary>
        IReadOnlyList<string> BootstrapServers { get; }

        /// <summary>
        /// The number of acknowledgments the producer requires the leader to have received before considering a request complete.
        /// This controls the durability of records that are sent.
        /// </summary>
        AcksType Acknowledgments { get; }

        /// <summary>
        /// When set to 'true', the producer will ensure that exactly one copy of each message is written in the stream.
        /// If 'false', producer retries due to broker failures, etc., may write duplicates of the retried message in the stream.
        /// </summary>
        int BatchSize { get; }

        /// <summary>
        /// When set to 'true', the producer will ensure that exactly one copy of each message is written in the stream.
        /// If 'false', producer retries due to broker failures, etc., may write duplicates of the retried message in the stream.
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Setting a value greater than zero will cause the client to resend any record whose send fails with a potentially transient error.
        /// Note that this retry is no different than if the client resent the record upon receiving the error.
        /// Allowing retries without setting max.in.flight.requests.per.connection to 1 will potentially change the ordering of records because
        /// if two batches are sent to a single partition, and the first fails and is retried but the second succeeds,
        /// then the records in the second batch may appear first.
        /// </summary>
        int Retries { get; }

        /// <summary>
        /// The maximum number of unacknowledged requests the client will send on a single connection before blocking.
        /// Note that if this setting is set to be greater than 1 and there are failed sends,
        /// there is a risk of message re-ordering due to retries (i.e., if retries are enabled).
        /// </summary>
        int MaxInFlightRequestPerConnection { get; }

        /// <summary>
        /// When set to 'true', the producer will ensure that exactly one copy of each message is written in the stream.
        /// If 'false', producer retries due to broker failures, etc., may write duplicates of the retried message in the stream.
        /// </summary>
        bool EnableIdempotence { get; }
    }
}