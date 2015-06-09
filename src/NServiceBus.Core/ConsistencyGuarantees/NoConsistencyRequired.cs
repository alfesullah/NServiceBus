namespace NServiceBus.ConsistencyGuarantees
{
    /// <summary>
    /// No consistency is required, the tranport is allowed to optimize then outgoing
    /// operaton as it sees fit
    /// </summary>
    public class NoConsistencyRequired : ConsistencyGuarantee
    {
        
    }
}