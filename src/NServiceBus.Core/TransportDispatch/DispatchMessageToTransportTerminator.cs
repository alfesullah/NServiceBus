namespace NServiceBus
{
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Transports;

    class DispatchMessageToTransportTerminator : PipelineTerminator<PhysicalOutgoingContextStageBehavior.Context>
    {
        readonly ISendMessages messageSender;

        public DispatchMessageToTransportTerminator(ISendMessages messageSender)
        {
            this.messageSender = messageSender;
        }

        public override void Terminate(PhysicalOutgoingContextStageBehavior.Context context)
        {
            var state = context.Extensions.GetOrCreate<State>();
            state.Headers[Headers.MessageId] = state.MessageId;

            var message = new OutgoingMessage(state.MessageId, state.Headers, context.Body);
            
            var requiredGuarantee = context.GetConsistencyGuarantee();

            var deliveryConstraints = context.GetDeliveryConstraints();

            var routingStrategy = context.Get<RoutingStrategy>();

            DispatchStrategy dispatchStrategy;

            if(!context.TryGet(out dispatchStrategy))
            {
                dispatchStrategy = new DefaultDispatchStrategy(messageSender);
            }

            dispatchStrategy.Dispatch(message, routingStrategy, requiredGuarantee, deliveryConstraints, context);
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

    class DefaultDispatchStrategy : DispatchStrategy
    {
        readonly ISendMessages messageSender;

        public DefaultDispatchStrategy(ISendMessages messageSender)
        {
            this.messageSender = messageSender;
        }

        public override void Dispatch(OutgoingMessage message, RoutingStrategy routingStrategy, ConsistencyGuarantee minimumConsistencyGuarantee, IEnumerable<DeliveryConstraint> constraints, BehaviorContext currentContext)
        {
            messageSender.Send(message, new TransportSendOptions(((DirectRoutingStrategy)routingStrategy).Destination,minimumConsistencyGuarantee,constraints,currentContext));
        }
    }

    abstract class DispatchStrategy
    {
        public abstract void Dispatch(OutgoingMessage message,
            RoutingStrategy routingStrategy,
            ConsistencyGuarantee minimumConsistencyGuarantee,
            IEnumerable<DeliveryConstraint> constraints,
            BehaviorContext currentContext);
    }
}