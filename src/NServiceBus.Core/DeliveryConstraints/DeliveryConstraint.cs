namespace NServiceBus.DeliveryConstraints
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public abstract class DeliveryConstraint
    {
        internal abstract bool Deserialize(Dictionary<string, string> options);

        internal abstract void Serialize(Dictionary<string, string> options);
    }
}