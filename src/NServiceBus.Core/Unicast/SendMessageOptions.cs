namespace NServiceBus.Unicast
{
    using System;

    /// <summary>
    /// Controls how a message will be sent by the transport
    /// </summary>
    public class SendMessageOptions : DeliveryMessageOptions
    {

        /// <summary>
        /// Creates an instance of <see cref="SendMessageOptions"/>.
        /// </summary>
        /// <param name="deliverAt">The time when the message should be delivered to the destination.</param>
        /// <param name="delayDeliveryFor">How long to delay delivery of the message.</param>
        public SendMessageOptions(DateTime? deliverAt = null, TimeSpan? delayDeliveryFor = null)
        {
            if (deliverAt != null && delayDeliveryFor != null)
            {
                throw new ArgumentException("Ensure you either set `deliverAt` or `delayDeliveryFor`, but not both.");
            }

            DeliverAt = deliverAt;

            Guard.AgainstNegative(delayDeliveryFor, "delayDeliveryFor");
            DelayDeliveryFor = delayDeliveryFor;
        }

        /// <summary>
        /// The time when the message should be delivered to the destination.
        /// </summary>
        public DateTime? DeliverAt { get; private set; }

        /// <summary>
        /// How long to delay delivery of the message.
        /// </summary>
        public TimeSpan? DelayDeliveryFor { get; private set; }
    }
}