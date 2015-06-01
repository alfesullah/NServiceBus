namespace NServiceBus.Transports.Msmq
{
    using System;
    using System.Collections.ObjectModel;
    using System.Messaging;
    using System.Transactions;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.Transports.Msmq.Config;
    using NServiceBus.Unicast.Queuing;

    /// <summary>
    /// Default MSMQ <see cref="ISendMessages"/> implementation.
    /// </summary>
    public class MsmqMessageSender : ISendMessages
    {
        /// <summary>
        /// Creates a new sender.
        /// </summary>
        /// <param name="settings">The current msmq settings</param>
        public MsmqMessageSender(MsmqSettings settings)
        {
            Guard.AgainstNull(settings, "settings");
            this.settings = settings;
        }

        /// <summary>
        /// Stores the value set by <see cref="MsmqConfigurationExtensions.ApplyLabelToMessages"/>
        /// </summary>
        public MsmqLabelGenerator MessageLabelConvention { get; set; }

        /// <summary>
        /// Sends the given <paramref name="message"/>
        /// </summary>
        public void Send(OutgoingMessage message, TransportSendOptions sendOptions)
        {
            Guard.AgainstNull(message, "message");
            Guard.AgainstNull(sendOptions, "sendOptions");
            var destination = sendOptions.Destination;
            var destinationAddress = MsmqAddress.Parse(destination);
            var queuePath = MsmqUtilities.GetFullPath(destinationAddress);
            try
            {
                using (var q = new MessageQueue(queuePath, false, settings.UseConnectionCache, QueueAccessMode.Send))
                using (var toSend = MsmqUtilities.Convert(message, sendOptions))
                {
                    toSend.UseDeadLetterQueue = settings.UseDeadLetterQueue;
                    toSend.UseJournalQueue = settings.UseJournalQueue;
                    toSend.TimeToReachQueue = settings.TimeToReachQueue;

                    string replyToAddress;

                    if (message.Headers.TryGetValue(Headers.ReplyToAddress, out replyToAddress))
                    {
                        var returnAddress = MsmqUtilities.GetReturnAddress(replyToAddress, destinationAddress.Machine);
                        toSend.ResponseQueue = new MessageQueue(returnAddress);
                    }

                    MessageQueueTransaction receiveTransaction;
                    sendOptions.Context.TryGet(out receiveTransaction);


                   
                    var label = GetLabel(message)

                    if (sendOptions.MimimumConsistencyGuarantee is AtomicWithReceiveOperation && receiveTransaction != null)
                    {
                        q.Send(toSend, label, receiveTransaction);
                    }
                    else
                    {
                        var transactionType = GetTransactionTypeForSend();

                        q.Send(toSend, label, transactionType);
                    }
                }
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
                {
                    var msg = destination == null
                                     ? "Failed to send message. Target address is null."
                                     : string.Format("Failed to send message to address: [{0}]", destination);

                    throw new QueueNotFoundException(destination, msg, ex);
                }

                ThrowFailedToSendException(destination, ex);
            }
            catch (Exception ex)
            {
                ThrowFailedToSendException(destination, ex);
            }
        }

        string GetLabel(OutgoingMessage message)
        {
            if (MessageLabelConvention == null)
            {
                return string.Empty;
            }
            var messageLabel = MessageLabelConvention(new ReadOnlyDictionary<string, string>(message.Headers));
            if (messageLabel == null)
            {
                throw new Exception("MSMQ label convention returned a null. Either return a valid value or a String.Empty to indicate 'no value'.");
            }
            if (messageLabel.Length > 240)
            {
                throw new Exception("MSMQ label convention returned a value longer than 240 characters. This is not supported.");
            }
            return messageLabel;
        }

        static void ThrowFailedToSendException(string address, Exception ex)
        {
            if (address == null)
                throw new Exception("Failed to send message.", ex);

            throw new Exception(
                string.Format("Failed to send message to address: {0}", address), ex);
        }

        MessageQueueTransactionType GetTransactionTypeForSend()
        {
            if (!settings.UseTransactionalQueues)
            {
                return MessageQueueTransactionType.None;
            }

            return Transaction.Current != null
                       ? MessageQueueTransactionType.Automatic
                       : MessageQueueTransactionType.Single;
        }

        MsmqSettings settings;
    }
}