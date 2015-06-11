namespace NServiceBus.DelayedDelivery
{
    using NServiceBus.Transports;

    class NoOpCanceling:ICancelDeferredMessages
    {
        public void CancelDeferredMessages(string messageKey)
        {
        }
    }
}