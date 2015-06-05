namespace NServiceBus.Features
{
    class HostInformationFeature:Feature
    {
        public HostInformationFeature()
        {
            EnableByDefault();
        }
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            context.Pipeline.Register("AuditHostInformation", typeof(AuditHostInformationBehavior), "Adds audit host information");
        
        }
    }
}