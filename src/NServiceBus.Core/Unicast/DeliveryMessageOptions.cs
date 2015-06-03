namespace NServiceBus.Unicast
{
    using System;

    /// <summary>
    /// Options to deliver messages.
    /// </summary>
    public partial class DeliveryMessageOptions
    {
        /// <summary>
        /// The TTBR to use for this message
        /// </summary>
        public TimeSpan? TimeToBeReceived { get; set; }

        /// <summary>
        /// Controls if the transport should be requested to handle the message in a way that it survives restarts
        /// </summary>
        public bool? NonDurable { get; set; }
    }
}