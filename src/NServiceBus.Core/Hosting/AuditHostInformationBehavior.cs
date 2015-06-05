namespace NServiceBus
{
    using System;
    using NServiceBus.Audit;
    using NServiceBus.Hosting;
    using NServiceBus.Pipeline;

    class AuditHostInformationBehavior : Behavior<AuditContext>
    {
        readonly HostInformation hostInfo;

        public AuditHostInformationBehavior(HostInformation hostInfo)
        {
            this.hostInfo = hostInfo;
        }

        public override void Invoke(AuditContext context, Action next)
        {
            context.AddAuditData(Headers.HostId, hostInfo.HostId.ToString("N"));
            context.AddAuditData(Headers.HostDisplayName,hostInfo.DisplayName);
            next();
        }
    }
}