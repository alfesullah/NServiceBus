namespace NServiceBus.Transports
{
    class DefaultMessageAuditer
    {
        public IDispatchMessages MessageSender { get; set; }

        public void Audit(DispatchOptions sendOptions, OutgoingMessage message)
        {
            MessageSender.Dispatch(message, sendOptions);
        }

        class Initialization : INeedInitialization
        {
            public void Customize(BusConfiguration configuration)
            {
                configuration.RegisterComponents(c => c.ConfigureComponent<DefaultMessageAuditer>(DependencyLifecycle.InstancePerCall));

                configuration.RegisterComponents(c => c.ConfigureComponent<AuditerWrapper>(DependencyLifecycle.InstancePerCall)
                    .ConfigureProperty(t => t.AuditerImplType, typeof(DefaultMessageAuditer)));
            }
        }
    }
}