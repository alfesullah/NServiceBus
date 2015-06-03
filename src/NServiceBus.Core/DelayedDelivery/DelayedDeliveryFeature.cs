namespace NServiceBus.Features
{
    using System;
    using NServiceBus.Config;
    using NServiceBus.DelayedDelivery;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Routing;
    using NServiceBus.Timeout;
    using NServiceBus.Transports;

    class DelayedDeliveryFeature:Feature
    {
        public DelayedDeliveryFeature()
        {
            EnableByDefault();
        }
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            if (!context.DoesTransportSupportConstraint<DelayedDeliveryConstraint>())
            {
                var timeoutManagerAddress = GetTimeoutManagerAddress(context);

                context.MainPipeline.Register("RouteDeferredMessageToTimeoutManager", typeof(RouteDeferredMessageToTimeoutManagerBehavior), "Reroutes deferred messages to the timeout manager");


                context.Container.ConfigureComponent(b => new RouteDeferredMessageToTimeoutManagerBehavior(timeoutManagerAddress), DependencyLifecycle.SingleInstance);

            }
            context.MainPipeline.Register("ApplyDelayedDeliveryConstraint", typeof(ApplyDelayedDeliveryConstraintBehavior), "Applied relevant delayed delivery constraints requested by the user");
        }

        static string GetTimeoutManagerAddress(FeatureConfigurationContext context)
        {
            var unicastConfig = context.Settings.GetConfigSection<UnicastBusConfig>();

            if (unicastConfig != null && !string.IsNullOrWhiteSpace(unicastConfig.TimeoutManagerAddress))
            {
                return unicastConfig.TimeoutManagerAddress;
            }
            var selectedTransportDefinition = context.Settings.Get<TransportDefinition>();
            return selectedTransportDefinition.GetSubScope(context.Settings.Get<string>("MasterNode.Address"), "Timeouts");
        }
    }

    class RouteDeferredMessageToTimeoutManagerBehavior:Behavior<OutgoingContext>
    {

        public override void Invoke(OutgoingContext context, Action next)
        {
            DelayedDeliveryConstraint constraint;

            if (context.TryGetDeliveryConstraint(out constraint))
            {
                var currentRoutingStrategy = context.Get<RoutingStrategy>() as DirectToTargetDestination;

                if (currentRoutingStrategy == null)
                {
                    throw new Exception("Delayed delivery using the timeoutmanager is only supported for messages with Direct routing");
                }

                context.Set<RoutingStrategy>(new DirectToTargetDestination(timeoutManagerAddress));


                DateTime deliverAt;
                var delayConstraint = constraint as DelayDeliveryWith;

                if (delayConstraint != null)
                {
                    deliverAt = DateTime.UtcNow + delayConstraint.Delay;
                }
                else
                {

                    throw new NotImplementedException();
                }

                context.SetHeader(TimeoutManagerHeaders.Expire,DateTimeExtensions.ToWireFormattedString(deliverAt));

                context.RemoveDeliveryConstaint(constraint);

            }

            next();
        }

        readonly string timeoutManagerAddress;

        public RouteDeferredMessageToTimeoutManagerBehavior(string timeoutManagerAddress)
        {
            this.timeoutManagerAddress = timeoutManagerAddress;
        }

        public class Registration:RegisterStep
        {


            public Registration()
                : base("RouteDeferredMessageToTimeoutManager", typeof(RouteDeferredMessageToTimeoutManagerBehavior), "Reroutes deferred messages to the timeout manager")
            {
                InsertAfter("ApplyDelayedDeliveryConstraint");
            }
        }
    }
}