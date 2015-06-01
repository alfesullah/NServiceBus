namespace NServiceBus.Transports
{
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;

    /// <summary>
    /// Contains details on how the message should be sent
    /// </summary>
    public class TransportSendOptions
    {
        /// <summary>
        /// Creates the send options with the given address
        /// </summary>
        /// <param name="destination">The native address where to sent this message</param>
        /// <param name="minimumConsistencyGuarantee">The level of consistency that's required for this operation</param>
        /// <param name="deliveryConstraints">The delivery constraints that must be honored by the transport</param>
        /// <param name="context">The pipeline context if present</param>
        public TransportSendOptions(string destination, ConsistencyGuarantee minimumConsistencyGuarantee, IEnumerable<DeliveryConstraint> deliveryConstraints,BehaviorContext context = null)
        {
            Destination = destination;
            MinimumConsistencyGuarantee = minimumConsistencyGuarantee;
            DeliveryConstraints = deliveryConstraints;
            Context = context;

            if (context == null)
            {
                Context = new RootContext(null);
            }
        }

        /// <summary>
        /// The address where this message should be sent to
        /// </summary>
        public string Destination { get; private set; }

        /// <summary>
        /// The level of consistency that's required for this operation
        /// </summary>
        public ConsistencyGuarantee MinimumConsistencyGuarantee { get; private set; }
        
        /// <summary>
        /// The delivery constraints that must be honored by the transport
        /// </summary>
        public IEnumerable<DeliveryConstraint> DeliveryConstraints { get; private set; }

        /// <summary>
        /// Access to the behavior context
        /// </summary>
        public BehaviorContext Context { get; private set; }
    }
}