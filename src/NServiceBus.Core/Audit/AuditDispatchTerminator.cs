namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.Audit;
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

            State state;
            
            if (context.TryGet(out state))
            {
                //transfer audit values to the headers of the messag to audit
                foreach (var kvp in state.AuditValues)
                {
                    message.Headers[kvp.Key] = kvp.Value;
                }
            }


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


        public class State
        {
            public Dictionary<string,string> AuditValues = new Dictionary<string, string>();  
        }
    }
}