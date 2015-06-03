namespace NServiceBus
{
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Routing;
    using NServiceBus.Transports;

    class DispatchMessageToTransportTerminator : PipelineTerminator<PhysicalOutgoingContextStageBehavior.Context>
    {
        readonly DispatchStrategy strategy;
        readonly IDispatchMessages dispatcher;

        public DispatchMessageToTransportTerminator(DispatchStrategy strategy, IDispatchMessages dispatcher)
        {
            this.strategy = strategy;
            this.dispatcher = dispatcher;
        }

        public override void Terminate(PhysicalOutgoingContextStageBehavior.Context context)
        {
            var state = context.Extensions.GetOrCreate<State>();
            state.Headers[Headers.MessageId] = state.MessageId;

            var message = new OutgoingMessage(state.MessageId, state.Headers, context.Body);
            
            var requiredGuarantee = context.GetConsistencyGuarantee();

            var deliveryConstraints = context.GetDeliveryConstraints();

            var routingStrategy = context.Get<RoutingStrategy>();


            strategy.Dispatch(dispatcher, message, routingStrategy, requiredGuarantee, deliveryConstraints, context);
        }
     
        public class State
        {
            public State()
            {
                Headers = new Dictionary<string, string>();
                MessageId = CombGuid.Generate().ToString();
            }
            public Dictionary<string, string> Headers { get; private set; }
            public string MessageId { get; set; }
        }
    }

    class DefaultDispatcher : DispatchStrategy
    {
        
        public override void Dispatch(IDispatchMessages dispatcher,OutgoingMessage message, RoutingStrategy routingStrategy, ConsistencyGuarantee minimumConsistencyGuarantee, IEnumerable<DeliveryConstraint> constraints, BehaviorContext currentContext)
        {
            dispatcher.Dispatch(message, new DispatchOptions(routingStrategy, minimumConsistencyGuarantee, constraints, currentContext));
        }
    }

    abstract class DispatchStrategy
    {
        public abstract void Dispatch(IDispatchMessages dispatcher,OutgoingMessage message,
            RoutingStrategy routingStrategy,
            ConsistencyGuarantee minimumConsistencyGuarantee,
            IEnumerable<DeliveryConstraint> constraints,
            BehaviorContext currentContext);
    }
}