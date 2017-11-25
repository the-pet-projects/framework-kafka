namespace PetProject.Framework.Kafka.Configurations
{
    public enum AcksType
    {
        /// <summary>
        /// The producer will not wait for any acknowledgment from the server at all.
        /// </summary>
        None = 0,

        /// <summary>
        /// The leader will write the record to its local log but will respond without awaiting full acknowledgement from all followers.
        /// In this case should the leader fail immediately after acknowledging the record but before the followers have replicated it 
        /// then the record will be lost.
        /// </summary>
        AtLeastOne = 1,

        /// <summary>
        /// This means the leader will wait for the full set of in-sync replicas to acknowledge the record.
        ///  This guarantees that the record will not be lost as long as at least one in-sync replica remains alive.
        ///  This is the strongest available guarantee.
        /// </summary>
        All = 2
    }
}