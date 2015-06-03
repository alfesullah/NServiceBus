namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.DeliveryConstraints;

    /// <summary>
    /// 
    /// </summary>
    public class DelayedDelivery : DeliveryConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        public DelayedDelivery(TimeSpan delay)
        {
            DelayDeliveryWith = delay;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doNotDeliverBefore"></param>
        public DelayedDelivery(DateTime doNotDeliverBefore)
        {
            DoNotDeliverBefore = doNotDeliverBefore;
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? DelayDeliveryWith { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime? DoNotDeliverBefore { get; private set; }

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