namespace NServiceBus.Routing.Publishing
{
    using System.Collections.Generic;
    using System.Linq;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.Transports;
    using NServiceBus.Unicast.Messages;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    class StorageDrivenDispatcher : DispatchStrategy
    {
        public StorageDrivenDispatcher(ISubscriptionStorage subscriptionStorage, MessageMetadataRegistry messageMetadataRegistry)
        {
            this.subscriptionStorage = subscriptionStorage;
            this.messageMetadataRegistry = messageMetadataRegistry;
        }


        public override void Dispatch(ISendMessages dispatcher,OutgoingMessage message, RoutingStrategy routingStrategy, ConsistencyGuarantee minimumConsistencyGuarantee, IEnumerable<DeliveryConstraint> constraints, BehaviorContext currentContext)
        {
            var eventType = ((ToAllSubscribers)routingStrategy).EventType;

            var eventTypesToPublish = messageMetadataRegistry.GetMessageMetadata(eventType)
                .MessageHierarchy
                .Distinct()
                .ToList();

            var subscribers = subscriptionStorage.GetSubscriberAddressesForMessage(eventTypesToPublish.Select(t => new MessageType(t))).ToList();

            if (!subscribers.Any())
            {
                currentContext.Set("NoSubscribersFoundForMessage", true);
                return;
            }

            currentContext.Set("SubscribersForEvent", subscribers);

            var currentConstraints = constraints.ToList();

            foreach (var subscriber in subscribers)
            {
                dispatcher.Send(message, new TransportSendOptions(subscriber,
                    minimumConsistencyGuarantee,
                   currentConstraints,
                    currentContext));
            }
        }

        readonly ISubscriptionStorage subscriptionStorage;
        readonly MessageMetadataRegistry messageMetadataRegistry;
    }
}