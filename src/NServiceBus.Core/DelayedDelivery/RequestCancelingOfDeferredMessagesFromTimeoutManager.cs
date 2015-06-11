namespace NServiceBus.DelayedDelivery
{
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Routing;
    using NServiceBus.Timeout;
    using NServiceBus.Transports;
    using NServiceBus.Unicast.Transport;

    class RequestCancelingOfDeferredMessagesFromTimeoutManager:ICancelDeferredMessages
    {
        IDispatchMessages dispatcher;
        string timeoutManagerAddress;

        public RequestCancelingOfDeferredMessagesFromTimeoutManager(string timeoutManagerAddress, IDispatchMessages dispatcher)
        {
            this.timeoutManagerAddress = timeoutManagerAddress;
            this.dispatcher = dispatcher;
        }

        public void CancelDeferredMessages(string messageKey)
        {
            var controlMessage = ControlMessageFactory.Create(MessageIntentEnum.Send);

            controlMessage.Headers[Headers.SagaId] = messageKey;
            controlMessage.Headers[TimeoutManagerHeaders.ClearTimeouts] = bool.TrueString;

            dispatcher.Dispatch(controlMessage, new DispatchOptions(new DirectToTargetDestination(timeoutManagerAddress),new AtomicWithReceiveOperation(), new List<DeliveryConstraint>(),null )); 
        }
    }
}