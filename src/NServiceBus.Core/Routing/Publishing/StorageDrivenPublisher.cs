namespace NServiceBus.Routing.Publishing
{
    using System.Linq;
    using NServiceBus.Transports;
    using NServiceBus.Unicast.Messages;
    using NServiceBus.Unicast.Subscriptions;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;

    class StorageDrivenPublisher:IPublishMessages
    {
        public StorageDrivenPublisher(ISubscriptionStorage subscriptionStorage, ISendMessages messageSender, MessageMetadataRegistry messageMetadataRegistry)
        {
            this.subscriptionStorage = subscriptionStorage;
            this.messageSender = messageSender;
            this.messageMetadataRegistry = messageMetadataRegistry;
        }


        public void Publish(OutgoingMessage message, TransportPublishOptions publishOptions)
        {
            var eventTypesToPublish = messageMetadataRegistry.GetMessageMetadata(publishOptions.EventType)
                .MessageHierarchy
                .Distinct()
                .ToList();

            var subscribers = subscriptionStorage.GetSubscriberAddressesForMessage(eventTypesToPublish.Select(t => new MessageType(t))).ToList();

            if (!subscribers.Any())
            {
                publishOptions.Context.Set("NoSubscribersFoundForMessage", true);
                return;
            }

            publishOptions.Context.Set("SubscribersForEvent", subscribers);

            foreach (var subscriber in subscribers)
            {
                messageSender.Send(message, new TransportSendOptions(subscriber,
                    publishOptions.MinimumConsistencyGuarantee,
                    publishOptions.DeliveryConstraints.ToList(),
                    publishOptions.Context));
            }
        }

        ISubscriptionStorage subscriptionStorage;
        ISendMessages messageSender;
        MessageMetadataRegistry messageMetadataRegistry;
    }
}