namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Outbox;
    using NServiceBus.Pipeline;
    using NServiceBus.Routing;
    using NServiceBus.TransportDispatch;
    using NServiceBus.Transports;

    class OutboxDeduplicationBehavior : PhysicalMessageProcessingStageBehavior
    {
        public OutboxDeduplicationBehavior(IOutboxStorage outboxStorage,
            TransactionOptions transactionOptions, 
            IDispatchMessages dispatcher,
            DispatchStrategy dispatchStrategy)
        {
            this.outboxStorage = outboxStorage;
            this.transactionOptions = transactionOptions;
            this.dispatcher = dispatcher;
            this.dispatchStrategy = dispatchStrategy;
        }

        public override void Invoke(Context context, Action next)
        {
            var messageId = context.PhysicalMessage.Id;
            OutboxMessage outboxMessage;

            if (!outboxStorage.TryGet(messageId, out outboxMessage))
            {
                outboxMessage = new OutboxMessage(messageId);

                //override the current dispatcher with to make sure all outgoing ops gets stored in the outbox
                context.Set<DispatchStrategy>(new OutboxDispatchStrategy(outboxMessage));

                //we use this scope to make sure that we escalate to DTC if the user is talking to another resource by misstake
                using (var checkForEscalationScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
                {
                    next();
                    checkForEscalationScope.Complete();
                }


                if (context.handleCurrentMessageLaterWasCalled)
                {
                    return;
                }

                outboxStorage.Store(messageId,outboxMessage.TransportOperations);
            }

            DispatchOperationToTransport(outboxMessage.TransportOperations,context);

            outboxStorage.SetAsDispatched(messageId);
        }

        void DispatchOperationToTransport(IEnumerable<TransportOperation> operations, Context context)
        {
            foreach (var transportOperation in operations)
            {
                var message = new OutgoingMessage(transportOperation.MessageId, transportOperation.Headers, transportOperation.Body);

                var routingStrategy = routingStrategyFactory.Create(transportOperation.Options);

           
                //todo: deliveryConstraint.Hydrate(transportOperation.Options);

                dispatchStrategy.Dispatch(dispatcher,message, routingStrategy, new AtLeastOnce(), new List<DeliveryConstraint>(), context);
            }
        }

        IDispatchMessages dispatcher;
        DispatchStrategy dispatchStrategy;
        IOutboxStorage outboxStorage;
        TransactionOptions transactionOptions;
        RoutingStrategyFactory routingStrategyFactory = new RoutingStrategyFactory();

        public class OutboxDeduplicationRegistration : RegisterStep
        {
            public OutboxDeduplicationRegistration()
                : base("OutboxDeduplication", typeof(OutboxDeduplicationBehavior), "Deduplication for the outbox feature")
            {
                InsertBeforeIfExists(WellKnownStep.AuditProcessedMessage);
            }
        }
    }
}