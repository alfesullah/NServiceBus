namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline;
    using NServiceBus.SecondLevelRetries;
    using NServiceBus.Transports;

    class SecondLevelRetriesBehavior : PhysicalMessageProcessingStageBehavior
    {
        public SecondLevelRetriesBehavior(ISendMessages messageSender, SecondLevelRetryPolicy retryPolicy, BusNotifications notifications)
        {
            this.messageSender = messageSender;
            this.retryPolicy = retryPolicy;
            this.notifications = notifications;
        }

        public override void Invoke(Context context, Action next)
        {
            try
            {
                next();
            }
            catch (MessageDeserializationException)
            {
                context.PhysicalMessage.Headers.Remove(Headers.Retries);
                throw; // no SLR for poison messages
            }
            catch (Exception ex)
            {
                var message = context.PhysicalMessage;
                var currentRetry = GetNumberOfRetries(message.Headers) +1;

                TimeSpan delay;

                if (retryPolicy.TryGetDelay(message, ex, currentRetry, out delay))
                {
                    var receiveAddress = PipelineInfo.PublicAddress;

                    message.Headers[Headers.Retries] = currentRetry.ToString();
                    message.Headers[RetriesTimestamp] = DateTimeExtensions.ToWireFormattedString(DateTime.UtcNow);

                    var sendOptions = new TransportSendOptions(receiveAddress,new AtomicWithReceiveOperation(), 
                        new List<DeliveryConstraint>
                    {
                        new DelayedDelivery(delay)
                    });

                    messageSender.Send(new OutgoingMessage(context.PhysicalMessage.Id, message.Headers, message.Body),sendOptions);
             
                    notifications.Errors.InvokeMessageHasBeenSentToSecondLevelRetries(currentRetry,message,ex);

                    return;
                }

                message.Headers.Remove(Headers.Retries);

                throw;
            }

        }

        static int GetNumberOfRetries(Dictionary<string, string> headers)
        {
            string value;
            if (headers.TryGetValue(Headers.Retries, out value))
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    return i;
                }
            }
            return 0;
        }


        readonly ISendMessages messageSender;
        readonly SecondLevelRetryPolicy retryPolicy;
        readonly BusNotifications notifications;

        public const string RetriesTimestamp = "NServiceBus.Retries.Timestamp";

        public class Registration : RegisterStep
        {
            public Registration()
                : base("SecondLevelRetries", typeof(SecondLevelRetriesBehavior), "Performs second level retries")
            {
                InsertBeforeIfExists("FirstLevelRetries");
                InsertBeforeIfExists("ReceivePerformanceDiagnosticsBehavior");
            }
        }

    }
}