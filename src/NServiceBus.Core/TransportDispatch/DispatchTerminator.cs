namespace NServiceBus
{
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.Routing;
    using NServiceBus.TransportDispatch;
    using NServiceBus.Transports;

    class DispatchTerminator : PipelineTerminator<DispatchContext>
    {
        readonly IDispatchMessages dispatcher;
        readonly DispatchStrategy dispatchStrategy;

        public DispatchTerminator(IDispatchMessages dispatcher, DispatchStrategy dispatchStrategy)
        {
            this.dispatcher = dispatcher;
            this.dispatchStrategy = dispatchStrategy;
        }

        public override void Terminate(DispatchContext context)
        {
            dispatchStrategy.Dispatch(dispatcher, context.Get<OutgoingMessage>(), context.Get<RoutingStrategy>(), context.GetConsistencyGuarantee(), context.GetDeliveryConstraints(), context);
        }
    }
}