namespace NServiceBus
{
    using System;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;

    class RouteMessageBehavior:Behavior<OutgoingContext>
    {
        //TransportDefinition definition;

        public override void Invoke(OutgoingContext context, Action next)
        {
            if (!context.IsPublish())
            {

                //string destination = "x";

                //var strategy = new DirectRoutingStrategy(destination, definition.GetSender());


                //var destination = options.Destination;

                //if (string.IsNullOrEmpty(destination))
                //{
                //    destination = GetDestinationForSend(messageType);
                //}

                //          void ValidateDestination(OutgoingContext context)
                //{
                //    var sendOptions = context.DeliveryMessageOptions as SendMessageOptions;

                //    if (sendOptions != null && string.IsNullOrWhiteSpace(sendOptions.Destination))
                //    {
                //        throw new InvalidOperationException("No destination specified for message: " + context.MessageType);
                //    }
                //}


            }

            if (context.IsReply())
            {
                //var sendOptions = new SendMessageOptions(MessageBeingProcessed.ReplyToAddress);
            }

            next();
        }


        //string GetDestinationForSend(Type messageType)
        //{
        //    var destinations = GetAtLeastOneAddressForMessageType(messageType);

        //    if (destinations.Count > 1)
        //    {
        //        throw new InvalidOperationException("Sends can only target one address.");
        //    }

        //    return destinations.SingleOrDefault();
        //}


        //var operationType = transportOperation.Options["Operation"];

        //switch (operationType)
        //{
        //    case "Audit":
        //        defaultAuditer.Audit(new TransportSendOptions(transportOperation.Options["Destination"],null,false,false), message);
        //        break;
        //    case "Send":
        //        defaultDispatcher.NativeSendOrDefer(transportOperation.Options["Destination"],deliveryOptions, message);
        //        break;
        //    case "Publish":

        //        var options= new TransportPublishOptions(Type.GetType(transportOperation.Options["EventType"]));

        //        defaultDispatcher.NativePublish(options, message);
        //        break;
        //    default:
        //        throw new InvalidOperationException("Unknown operation type: " + operationType);
        //}

        public class State
        {
            public string Destination{ get; set; }
            public bool RouteToLocalInstance { get; set; }
        }


    }








       //public void NativePublish(TransportPublishOptions publishOptions, OutgoingMessage message)
       // {
       //     SetTransportHeaders(publishOptions.TimeToBeReceived, publishOptions.NonDurable, message);

       //     try
       //     {
       //         Publish(message, publishOptions);
       //     }
       //     catch (QueueNotFoundException ex)
       //     {
       //         var messageDescription = "ControlMessage";

       //         string enclosedMessageTypes;

       //         if (message.Headers.TryGetValue(Headers.EnclosedMessageTypes, out enclosedMessageTypes))
       //         {
       //             messageDescription = enclosedMessageTypes;
       //         }
       //         throw new Exception(string.Format("The destination queue '{0}' could not be found. You may have misconfigured the destination for this kind of message ({1}) in the MessageEndpointMappings of the UnicastBusConfig section in your configuration file. " + "It may also be the case that the given queue just hasn't been created yet, or has been deleted.", ex.Queue, messageDescription), ex);
       //     }
       // }

       // public void NativeSendOrDefer(string destination,DeliveryMessageOptions deliveryMessageOptions, OutgoingMessage message)
       // {
       //     SetTransportHeaders(deliveryMessageOptions.TimeToBeReceived, deliveryMessageOptions.NonDurable, message);

       //     try
       //     {
       //         SendOrDefer(destination,message, deliveryMessageOptions as SendMessageOptions);
       //     }
       //     catch (QueueNotFoundException ex)
       //     {
       //         var messageDescription = "ControlMessage";

       //         string enclosedMessageTypes;

       //         if (message.Headers.TryGetValue(Headers.EnclosedMessageTypes, out enclosedMessageTypes))
       //         {
       //             messageDescription = enclosedMessageTypes;
       //         }
       //         throw new Exception(string.Format("The destination queue '{0}' could not be found. You may have misconfigured the destination for this kind of message ({1}) in the MessageEndpointMappings of the UnicastBusConfig section in your configuration file. " + "It may also be the case that the given queue just hasn't been created yet, or has been deleted.", ex.Queue, messageDescription), ex);
       //     }
       // }

       // void SetTransportHeaders(TimeSpan? timeToBeReceived, bool? nonDurable, OutgoingMessage message)
       // {
       //     message.Headers[Headers.MessageId] = message.MessageId;


       //     if (timeToBeReceived.HasValue)
       //     {
       //         message.Headers[Headers.TimeToBeReceived] = timeToBeReceived.Value.ToString("c");
       //     }

       //     if (nonDurable.HasValue && nonDurable.Value)
       //     {
       //         message.Headers[Headers.NonDurableMessage] = true.ToString();
       //     }
       // }

       // void SendOrDefer(string destination,OutgoingMessage message, SendMessageOptions options)
       // {
       //     if ((options.DelayDeliveryFor.HasValue && options.DelayDeliveryFor > TimeSpan.Zero) ||
       //         (options.DeliverAt.HasValue && options.DeliverAt.Value.ToUniversalTime() > DateTime.UtcNow))
       //     {
       //         SetIsDeferredHeader(message.Headers);
       //         MessageDeferral.Defer(message, new TransportDeferOptions(
       //             destination,
       //             options.DelayDeliveryFor,
       //             options.DeliverAt,
       //             options.NonDurable ?? true,
       //             options.EnlistInReceiveTransaction));

       //         return;
       //     }

       //     MessageSender.Send(message, new TransportSendOptions(destination,
       //                                                             options.TimeToBeReceived,
       //                                                             options.NonDurable ?? true,
       //                                                             options.EnlistInReceiveTransaction));
       // }

       // static void SetIsDeferredHeader(Dictionary<string, string> headers)
       // {
       //     headers[Headers.IsDeferredMessage] = true.ToString();
       // }

       // void Publish(OutgoingMessage message, TransportPublishOptions publishOptions)
       // {
       //     if (MessagePublisher == null)
       //     {
       //         throw new InvalidOperationException("No message publisher has been registered. If you're using a transport without native support for pub/sub please enable the message driven publishing feature by calling config.EnableFeature<MessageDrivenSubscriptions>() in your configuration");
       //     }
       //     MessagePublisher.Publish(message, publishOptions);
       // }
}