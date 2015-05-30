namespace NServiceBus.Performance.TimeToBeReceived
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.DeliveryConstraints;

    /// <summary>
    /// 
    /// </summary>
    public class DiscardIfNotReceivedBefore : DeliveryConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan MaxTime { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxTime"></param>
        public DiscardIfNotReceivedBefore(TimeSpan maxTime)
        {
            MaxTime = maxTime;
        }

        internal override bool Deserialize(Dictionary<string, string> options)
        {
            string value;

            if (options.TryGetValue("TimeToBeReceived",out value))
            {
                MaxTime = TimeSpan.Parse(value);
                return true;
            }

            return false;
        }

        internal override void Serialize(Dictionary<string, string> options)
        {
            options["TimeToBeReceived"] = MaxTime.ToString();
           
        }
    }
}