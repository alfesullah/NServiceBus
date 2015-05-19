namespace NServiceBus
{
    using System;
    using NServiceBus.Outbox;
    using NServiceBus.Pipeline.Contexts;

    class OutboxSendBehavior : PhysicalOutgoingContextStageBehavior
    {
        public override void Invoke(Context context, Action next)
        {
            OutboxMessage currentOutboxMessage;

            if (context.TryGet(out currentOutboxMessage))
            {
                var options = context.DeliveryMessageOptions.ToTransportOperationOptions();

                var routingStrategy = context.Get<RoutingStrategy>();

                routingStrategy.Dehydrate(options);

                context.Set<RoutingStrategy>(new OutboxRoutingStrategy(currentOutboxMessage, options));
            }

            next();
        }
    }
}