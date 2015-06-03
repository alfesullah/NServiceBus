namespace NServiceBus.Outbox
{
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.Transports;

    class OutboxAwareAuditer
    {
        DefaultMessageAuditer defaultMessageAuditer;
        BehaviorContext behaviorContext;

        public OutboxAwareAuditer(DefaultMessageAuditer defaultMessageAuditer, BehaviorContext behaviorContext)
        {
            this.defaultMessageAuditer = defaultMessageAuditer;
            this.behaviorContext = behaviorContext;
        }

        public void Audit( string destination, OutgoingMessage message)
        {
            OutboxMessage currentOutboxMessage;

            if (behaviorContext.TryGet(out currentOutboxMessage))
            {
                var options = new Dictionary<string,string>();

                options["Operation"] = "Audit";

                options["Destination"] = destination;

                currentOutboxMessage.TransportOperations.Add(new TransportOperation(message.MessageId,options, message.Body, message.Headers));
            }
            else
            {
                defaultMessageAuditer.Audit(new DispatchOptions(destination,new AtomicWithReceiveOperation(), new List<DeliveryConstraint>()), message);
            }
        }
    }
}