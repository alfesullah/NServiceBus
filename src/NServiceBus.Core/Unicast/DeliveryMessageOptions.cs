namespace NServiceBus.Unicast
{
    /// <summary>
    /// Options to deliver messages.
    /// </summary>
    public partial class DeliveryMessageOptions
    {
        /// <summary>
        /// Controls if the transport should be requested to handle the message in a way that it survives restarts
        /// </summary>
        public bool? NonDurable { get; set; }
    }
}