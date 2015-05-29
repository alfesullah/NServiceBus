namespace NServiceBus.ConsistencyGuarantees
{
    using NServiceBus.Pipeline.Contexts;

    static class ConsistencyGuaranteeExtensions
    {
        public static ConsistencyGuarantee GetConsistencyGuarantee(this PhysicalOutgoingContextStageBehavior.Context context)
        {
            ConsistencyGuarantee guarantee;

            if (context.TryGet(out guarantee))
            {
                return guarantee;
            }

            return new AtomicWithReceiveOperation();
        }
    }
}