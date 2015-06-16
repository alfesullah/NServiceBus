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
        public DispatchTerminator(IDispatchMessages dispatcher, DispatchStrategy defaultDispatchStrategy)
        {
            this.dispatcher = dispatcher;
            this.defaultDispatchStrategy = defaultDispatchStrategy;
        }

        public override void Terminate(DispatchContext context)
        {
            DispatchStrategy dispatchStrategy;

            if (!context.TryGet(out dispatchStrategy))
            {
                dispatchStrategy = defaultDispatchStrategy;
            }

            dispatchStrategy.Dispatch(dispatcher, context.Get<OutgoingMessage>(), context.Get<RoutingStrategy>(), context.GetConsistencyGuarantee(), context.GetDeliveryConstraints(), context);
        }

        IDispatchMessages dispatcher;
        DispatchStrategy defaultDispatchStrategy;
    }
}