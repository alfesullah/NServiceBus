namespace NServiceBus.Features
{
    using NServiceBus.Audit;
    using NServiceBus.Pipeline;
    using NServiceBus.TransportDispatch;
    using NServiceBus.Transports;
    using NServiceBus.Unicast.Queuing.Installers;

    /// <summary>
    /// Enabled message auditing for this endpoint.
    /// </summary>
    public class Audit : Feature
    {
        internal Audit()
        {
            EnableByDefault();
            Prerequisite(config =>AuditConfigReader.GetConfiguredAuditQueue(config.Settings, out auditConfig),"No configured audit queue was found");
        }


        /// <summary>
        /// See <see cref="Feature.Setup"/>
        /// </summary>
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            context.Pipeline.Register<InvokeAuditPipelineBehavior.Registration>();
            context.Pipeline.Register("AuditDispatch", typeof(AuditDispatchTerminator), "Dispatches the audit message to the transport");
         
            context.Container.ConfigureComponent(b =>
            {
                var pipelinesCollection = context.Settings.Get<PipelineConfiguration>();
                var auditPipeline = new PipelineBase<AuditContext>(b, context.Settings, pipelinesCollection.MainPipeline);

                return new InvokeAuditPipelineBehavior(auditPipeline,auditConfig.Address);
            }, DependencyLifecycle.InstancePerCall);


            context.Container.ConfigureComponent(b => new AuditDispatchTerminator(b.Build<DispatchStrategy>(), b.Build<IDispatchMessages>(), auditConfig.TimeToBeReceived), DependencyLifecycle.SingleInstance);

            //context.Pipeline.Register<AuditBehavior.Registration>();
            //context.Pipeline.Register<AttachCausationHeadersBehavior.Registration>();

            context.Container.ConfigureComponent<AuditQueueCreator>(DependencyLifecycle.InstancePerCall)
                .ConfigureProperty(p => p.Enabled, true)
                .ConfigureProperty(t => t.AuditQueue, auditConfig.Address);
        }

        AuditConfigReader.Result auditConfig;
    }
}