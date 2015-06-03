namespace NServiceBus
{
    using System;
    using NServiceBus.DelayedDelivery;
    using NServiceBus.Extensibility;

    /// <summary>
    /// Provides ways for the end user to request delayed delivery of their messages
    /// </summary>
    public static class DelayedDeliveryOptionExtensions
    {
        /// <summary>
        /// Delays the delivery of the message with the specified delay
        /// </summary>
        /// <param name="options">The options being extended</param>
        /// <param name="delay">The requested delay</param>
        public static void DelayDeliveryWith(this SendLocalOptions options, TimeSpan delay)
        {
            Guard.AgainstNegativeAndZero(delay,"delay");

            options.GetExtensions().Set(new  ApplyDelayedDeliveryConstraintBehavior.State(new DelayDeliveryWith(delay)));
        }
        /// <summary>
        /// Requests that the message should not be delivered before the specified time
        /// </summary>
        /// <param name="options">The options being extended</param>
        /// <param name="at">The time when this message should be made available</param>
        public static void DoNotDeliveryBefore(this SendLocalOptions options, DateTime at)
        {
            
        }

        /// <summary>
        /// Delays the delivery of the message with the specified delay
        /// </summary>
        /// <param name="options">The options being extended</param>
        /// <param name="delay">The requested delay</param>
        public static void DelayDeliveryWith(this SendOptions options, TimeSpan delay)
        {
            Guard.AgainstNegativeAndZero(delay, "delay");

            options.GetExtensions().Set(new ApplyDelayedDeliveryConstraintBehavior.State(new DelayDeliveryWith(delay)));
        }
        /// <summary>
        /// Requests that the message should not be delivered before the specified time
        /// </summary>
        /// <param name="options">The options being extended</param>
        /// <param name="at">The time when this message should be made available</param>
        public static void DoNotDeliveryBefore(this SendOptions options, DateTime at)
        {

        }
        
    }

    //if (deliverAt != null && delayDeliveryFor != null)
      //      {
      //          throw new ArgumentException("Ensure you either set `deliverAt` or `delayDeliveryFor`, but not both.");
      //      }

      //string delayDeliveryForString;
      //          TimeSpan? delayDeliveryFor = null;
      //          if (options.TryGetValue("DelayDeliveryFor", out delayDeliveryForString))
      //          {
      //              delayDeliveryFor = TimeSpan.Parse(delayDeliveryForString);
      //          }

      //          string deliverAtString;
      //          DateTime? deliverAt = null;
      //          if (options.TryGetValue("DeliverAt", out deliverAtString))
      //          {
      //              deliverAt = DateTimeExtensions.ToUtcDateTime(deliverAtString);
      //          }
}