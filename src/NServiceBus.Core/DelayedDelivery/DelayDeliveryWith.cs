namespace NServiceBus.DelayedDelivery
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class DelayDeliveryWith : DelayedDeliveryConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        public DelayDeliveryWith(TimeSpan delay)
        {
            Delay = delay;
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Delay { get; private set; }


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