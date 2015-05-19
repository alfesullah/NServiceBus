namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.Outbox;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Transports;

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
                //if (context.IsPublish())
                //{
                //    options["Operation"] = "Publish";
                //    options["EventType"] = context.MessageType.AssemblyQualifiedName;
                //}
                //else
                //{
                //    var sendOptions = (SendMessageOptions)context.DeliveryMessageOptions;

                //    options["Operation"] = "Send";
                //    options["Destination"] = "todo";

                //    if (sendOptions.DelayDeliveryFor.HasValue)
                //    {
                //        options["DelayDeliveryFor"] = sendOptions.DelayDeliveryFor.Value.ToString();
                //    }

                //    if (sendOptions.DeliverAt.HasValue)
                //    {
                //        options["DeliverAt"] = DateTimeExtensions.ToWireFormattedString(sendOptions.DeliverAt.Value);
                //    }

                //}

            }

            next();
        }
    }

    class OutboxRoutingStrategy : RoutingStrategy
    {
        OutboxMessage currentOutboxMessage;
        Dictionary<string, string> options;

        public OutboxRoutingStrategy(OutboxMessage currentOutboxMessage, Dictionary<string, string> options)
        {
            this.currentOutboxMessage = currentOutboxMessage;
            this.options = options;
        }

        public override void Dispatch(OutgoingMessage message)
        {
            currentOutboxMessage.TransportOperations.Add(new TransportOperation(message.MessageId, options, message.Body, message.Headers));
        }
    }
}