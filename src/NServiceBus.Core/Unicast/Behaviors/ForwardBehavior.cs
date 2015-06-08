namespace NServiceBus
{
    using System;
    using NServiceBus.Pipeline;

    class ForwardBehavior : PhysicalMessageProcessingStageBehavior
    {
        public string ForwardReceivedMessagesTo { get; set; }


        public override void Invoke(Context context, Action next)
        {
            next();

            context.PhysicalMessage.RevertToOriginalBodyIfNeeded();

            throw new NotImplementedException("Will soon add a forwarding pipeline");

        //    MessageAuditer.Audit(new OutgoingMessage(context.PhysicalMessage.Id,context.PhysicalMessage.Headers,context.PhysicalMessage.Body),
        //        new DispatchOptions( ForwardReceivedMessagesTo,new AtomicWithReceiveOperation(), new List<DeliveryConstraint>()));
        }

        public class Registration : RegisterStep
        {
            public Registration()
                : base("ForwardMessageTo", typeof(ForwardBehavior), "Forwards message to the specified queue in the UnicastBus config section.")
            {
                InsertBefore(WellKnownStep.ExecuteUnitOfWork);
            }
        }
    }
}