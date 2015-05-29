namespace NServiceBus.Features
{
    using System.Linq;
    using NServiceBus.Config;
    using NServiceBus.Transports;
    using NServiceBus.Unicast.Routing;

    class RoutingFeature:Feature
    {
        public RoutingFeature()
        {
            EnableByDefault();
        }
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            context.MainPipeline.Register("DetermineRoutingForMessage", typeof(DetermineRoutingForMessageBehavior), "Determines how the message being sent should be routed");

            context.Container.ConfigureComponent(b => new DetermineRoutingForMessageBehavior(b.Build<ISendMessages>(),context.Settings.LocalAddress()), DependencyLifecycle.InstancePerCall);

            SetupStaticRouter(context);
        }

        static void SetupStaticRouter(FeatureConfigurationContext context)
        {
            var conventions = context.Settings.Get<Conventions>();

            var knownMessages = context.Settings.GetAvailableTypes()
                .Where(conventions.IsMessageType)
                .ToList();

            var unicastConfig = context.Settings.GetConfigSection<UnicastBusConfig>();
            var router = new StaticMessageRouter(knownMessages);

            if (unicastConfig != null)
            {
                var messageEndpointMappings = unicastConfig.MessageEndpointMappings.Cast<MessageEndpointMapping>()
                    .OrderByDescending(m => m)
                    .ToList();

                foreach (var mapping in messageEndpointMappings)
                {
                    mapping.Configure((messageType, address) =>
                    {
                        if (!(conventions.IsMessageType(messageType) || conventions.IsEventType(messageType) || conventions.IsCommandType(messageType)))
                        {
                            return;
                        }

                        if (conventions.IsEventType(messageType))
                        {
                            router.RegisterEventRoute(messageType, address);
                            return;
                        }

                        router.RegisterMessageRoute(messageType, address);
                    });
                }
            }


            context.Container.RegisterSingleton(router);
        }
    }
}