namespace NServiceBus
{
    using System.Collections.Generic;
    using System.Linq;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Outbox;
    using NServiceBus.Transports;

    class OutboxRoutingStrategy : RoutingStrategy
    {
        OutboxMessage currentOutboxMessage;
        Dictionary<string, string> options;

        public OutboxRoutingStrategy(OutboxMessage currentOutboxMessage, Dictionary<string, string> options)
        {
            this.currentOutboxMessage = currentOutboxMessage;
            this.options = options;
        }

        public override void Dispatch(OutgoingMessage message, ConsistencyGuarantee minimumConsistencyGuarantee, IEnumerable<DeliveryConstraint> constraints)
        {
            constraints.ToList().ForEach(c=>c.Serialize(options));
            currentOutboxMessage.TransportOperations.Add(new TransportOperation(message.MessageId, options, message.Body, message.Headers));          
        }
    }
}