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
}