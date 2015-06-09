namespace NServiceBus
{
    using System;
    using NServiceBus.Outbox;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.TransportDispatch;

    class OutboxSendBehavior : PhysicalOutgoingContextStageBehavior
    {
        public override void Invoke(Context context, Action next)
        {
            OutboxMessage currentOutboxMessage;

            if (context.TryGet(out currentOutboxMessage))
            {  
                context.Set<DispatchStrategy>(new OutboxDispatchStrategy(currentOutboxMessage));
            }

            next();
        }
    }
}