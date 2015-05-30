namespace NServiceBus.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NServiceBus.Performance.TimeToBeReceived;

    class TimeToBeReceived:Feature
    {
        public TimeToBeReceived()
        {
            EnableByDefault();
        }
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            context.MainPipeline.Register("ApplyTimeToBeReceived", typeof(ApplyTimeToBeReceivedBehavior), "Adds the `DiscardIfNotReceivedBefore` constraint to relevant messages");

            var mappings = GetMappings(context);

            context.Container.ConfigureComponent(b=>mappings,DependencyLifecycle.SingleInstance);
        }

        TimeToBeReceivedMappings GetMappings(FeatureConfigurationContext context)
        {
            var mappings = new Dictionary<Type, TimeSpan>();

            var knownMessages = context.Settings.GetAvailableTypes()
                .Where(context.Settings.Get<Conventions>().IsMessageType)
                .ToList();

            var convention = DefaultConvention;

            UserDefinedTimeToBeReceivedConvention userDefinedConvention;
            if (context.Settings.TryGet(out userDefinedConvention))
            {
                convention = userDefinedConvention.GetTimeToBeReceivedForMessage;
            }

            foreach (var messageType in knownMessages)
            {
                var timeToBeReceived = convention(messageType);

                if (timeToBeReceived < TimeSpan.MaxValue)
                {
                    mappings[messageType] = timeToBeReceived;
                }
            }
            return new TimeToBeReceivedMappings(mappings);
        }

        Func<Type, TimeSpan> DefaultConvention = t =>
        {
            var attributes = t.GetCustomAttributes(typeof(TimeToBeReceivedAttribute), true)
                .Select(s => s as TimeToBeReceivedAttribute)
                .ToList();

            return attributes.Count > 0 ? attributes.Last().TimeToBeReceived : TimeSpan.MaxValue;
        };

    }
}