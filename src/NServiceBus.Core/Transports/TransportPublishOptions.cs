namespace NServiceBus.Transports
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains details on how the message should be published
    /// </summary>
    public class TransportPublishOptions
    {
        /// <summary>
        /// Creates the send options with the given address
        /// </summary>
        /// <param name="eventType">The type of event being published</param>
        /// <param name="mimimumConsistencyGuarantee">The level of consitency that's required for this operation</param>
        /// <param name="deliveryConstraints">The delivery constraints that must be honored by the transport</param>
        public TransportPublishOptions(Type eventType, ConsistencyGuarantee mimimumConsistencyGuarantee, List<DeliveryConstraint> deliveryConstraints)
        {
            EventType = eventType;
            MimimumConsistencyGuarantee = mimimumConsistencyGuarantee;
            DeliveryConstraints = deliveryConstraints;
        }

        /// <summary>
        /// The type of event being published
        /// </summary>
        public Type EventType { get; private set; }

        /// <summary>
        /// The level of consitency that's required for this operation
        /// </summary>
        public ConsistencyGuarantee MimimumConsistencyGuarantee { get; private set; }

        /// <summary>
        /// The delivery constraints that must be honored by the transport
        /// </summary>
        public IEnumerable<DeliveryConstraint> DeliveryConstraints { get; private set; }
    }
}