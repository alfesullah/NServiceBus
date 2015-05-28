namespace NServiceBus.Transports
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains details on how the message should be sent
    /// </summary>
    public class TransportSendOptions
    {
        /// <summary>
        /// Creates the send options with the given address
        /// </summary>
        /// <param name="destination">The native address where to sent this message</param>
        /// <param name="mimimumConsistencyGuarantee">The level of consitency that's required for this operation</param>
        /// <param name="deliveryConstraints">The delivery constraints that must be honored by the transport</param>
        public TransportSendOptions(string destination, ConsistencyGuarantee mimimumConsistencyGuarantee, List<DeliveryConstraint> deliveryConstraints)
        {
            Destination = destination;
            MimimumConsistencyGuarantee = mimimumConsistencyGuarantee;
            DeliveryConstraints = deliveryConstraints;
        }

        /// <summary>
        /// The address where this message should be sent to
        /// </summary>
        public string Destination { get; private set; }

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