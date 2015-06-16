namespace NServiceBus.DelayedDelivery
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represent a constraint that the message can't be made available for consumption before a given time
    /// </summary>
    public class DoNotDeliverBefore : DelayedDeliveryConstraint
    {
        /// <summary>
        /// The actual time when the message can be available to the recipient
        /// </summary>
        public DateTime At { get; private set; }

        /// <summary>
        /// Initializes the constraint
        /// </summary>
        /// <param name="at">The earliest time this message should be made available to its consumers</param>
        public DoNotDeliverBefore(DateTime at)
        {
            At = at;
        }

        /// <summary>
        /// Serializes the constraint into the passed dictionary
        /// </summary>
        /// <param name="options">Dictionary where to store the data</param>
        public override void Serialize(Dictionary<string, string> options)
        {
            options["DeliverAt"] = DateTimeExtensions.ToWireFormattedString(At);
        }
    }
}