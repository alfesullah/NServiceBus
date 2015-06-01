namespace NServiceBus
{
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.Transports;

    class DirectRoutingStrategy : RoutingStrategy
    {
        ISendMessages messageSender;
        readonly string destination;
       
        public DirectRoutingStrategy(ISendMessages messageSender, string destination)
        {
            this.messageSender = messageSender;
            this.destination = destination;
        }

        public override void Dispatch(OutgoingMessage message, 
            ConsistencyGuarantee minimumConsistencyGuarantee, 
            IEnumerable<DeliveryConstraint> constraints,
            BehaviorContext currentContext
            )
        {
            messageSender.Send(message, new TransportSendOptions(destination, minimumConsistencyGuarantee, constraints,currentContext));         
        }
    }
}