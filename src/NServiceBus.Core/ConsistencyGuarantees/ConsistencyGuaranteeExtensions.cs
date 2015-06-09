namespace NServiceBus.ConsistencyGuarantees
{
    using NServiceBus.TransportDispatch;

    static class ConsistencyGuaranteeExtensions
    {
        public static ConsistencyGuarantee GetConsistencyGuarantee(this DispatchContext context)
        {
            ConsistencyGuarantee guarantee;

            if (context.TryGet(out guarantee))
            {
                return guarantee;
            }

            //todo: we need to get this default from the transport
            return new AtomicWithReceiveOperation();
        }
    }
}