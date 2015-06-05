namespace NServiceBus.Features
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.Hosting;

    class HostInformationFeature : Feature
    {
        const string HostIdSettingsKey = "NServiceBus.HostInformation.HostId";

        public HostInformationFeature()
        {
            EnableByDefault();
        }
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            var hostInformation = new HostInformation(context.Settings.Get<Guid>(HostIdSettingsKey),
                        context.Settings.Get<string>("NServiceBus.HostInformation.DisplayName"),
                        context.Settings.Get<Dictionary<string, string>>("NServiceBus.HostInformation.Properties"));

            context.Container.RegisterSingleton(hostInformation);

            context.Pipeline.Register("AuditHostInformation", typeof(AuditHostInformationBehavior), "Adds audit host information");

            context.Pipeline.Register("AddHostInfoHeaders", typeof(AddHostInfoHeadersBehavior), "Adds host info headers to outgoing headers");

            context.Container.ConfigureComponent(b => new AddHostInfoHeadersBehavior(hostInformation, context.Settings.EndpointName()), DependencyLifecycle.SingleInstance);
        }
    }
}