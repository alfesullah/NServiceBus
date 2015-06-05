namespace NServiceBus.Audit
{
    using NServiceBus.Pipeline;

    /// <summary>
    /// Provide context to behaviors on the audit pipeline
    /// </summary>
    public class AuditContext : BehaviorContext
    {
        /// <summary>
        /// Initializes the context
        /// </summary>
        /// <param name="parent">The parent incoming context</param>
        public AuditContext(PhysicalMessageProcessingStageBehavior.Context parent)
            : base(parent)
        {
        }
    }
}