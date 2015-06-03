namespace NServiceBus.Features
{
    using System;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Routing.Publishing;

    class EnableStorageDrivenPublishingBehavior:Behavior<OutgoingContext>
    {
        readonly StorageDrivenDispatcher dispatcher;

        public EnableStorageDrivenPublishingBehavior(StorageDrivenDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public override void Invoke(OutgoingContext context, Action next)
        {
            context.OverrideDispatchStrategy(dispatcher);
            next();
        }
    }
}