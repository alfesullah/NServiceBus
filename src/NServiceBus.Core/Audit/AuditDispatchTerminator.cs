namespace NServiceBus.Audit
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Performance.TimeToBeReceived;
    using NServiceBus.Pipeline;
    using NServiceBus.Routing;
    using NServiceBus.Transports;

    class AuditDispatchTerminator : PipelineTerminator<AuditContext>
    {
        public AuditDispatchTerminator(DispatchStrategy strategy, IDispatchMessages dispatcher,TimeSpan? timeToBeReceived)
        {
            this.strategy = strategy;
            this.dispatcher = dispatcher;
            this.timeToBeReceived = timeToBeReceived;
        }

        public override void Terminate(AuditContext context)
        {
            var message = context.Get<OutgoingMessage>();


            var routingStrategy = context.Get<RoutingStrategy>();

            var deliveryConstraints = new List<DeliveryConstraint>();

            if (timeToBeReceived.HasValue)
            {
                deliveryConstraints.Add(new DiscardIfNotReceivedBefore(timeToBeReceived.Value));
            }


            strategy.Dispatch(dispatcher, message, routingStrategy, new AtomicWithReceiveOperation(), deliveryConstraints, context);
        }

        readonly DispatchStrategy strategy;
        readonly IDispatchMessages dispatcher;
        readonly TimeSpan? timeToBeReceived;
    }
}