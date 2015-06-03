namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.DeliveryConstraints;

    /// <summary>
    /// 
    /// </summary>
    public class NonDurableDelivery : DeliveryConstraint
    {
        internal override bool Deserialize(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }

        internal override void Serialize(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }
    }
}