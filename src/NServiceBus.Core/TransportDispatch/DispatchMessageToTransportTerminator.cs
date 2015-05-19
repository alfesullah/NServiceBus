namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Transports;
    using NServiceBus.Unicast;
    using NServiceBus.Unicast.Queuing;

   
    class DispatchMessageToTransportTerminator : PipelineTerminator<PhysicalOutgoingContextStageBehavior.Context>
    {
        public ISendMessages MessageSender { get; set; }

        public IPublishMessages MessagePublisher { get; set; }

        public IDeferMessages MessageDeferral { get; set; }


        public override void Terminate(PhysicalOutgoingContextStageBehavior.Context context)
        {
            var state = context.Extensions.GetOrCreate<State>();
            var intent = MessageIntentEnum.Send;

            if (context.IsReply())
            {
                intent = MessageIntentEnum.Reply;
            }

            if (context.IsPublish())
            {
                intent = MessageIntentEnum.Publish;
            }

            state.Headers[Headers.MessageIntent] = intent.ToString();

            var message= new OutgoingMessage(state.MessageId, state.Headers, context.Body);

            context.Get<RoutingStrategy>()
                .Dispatch(message);
        }
      
        public void NativePublish(TransportPublishOptions publishOptions, OutgoingMessage message)
        {
            SetTransportHeaders(publishOptions.TimeToBeReceived, publishOptions.NonDurable, message);

            try
            {
                Publish(message, publishOptions);
            }
            catch (QueueNotFoundException ex)
            {
                var messageDescription = "ControlMessage";

                string enclosedMessageTypes;

                if (message.Headers.TryGetValue(Headers.EnclosedMessageTypes, out enclosedMessageTypes))
                {
                    messageDescription = enclosedMessageTypes;
                }
                throw new Exception(string.Format("The destination queue '{0}' could not be found. You may have mis-configured the destination for this kind of message ({1}) in the MessageEndpointMappings of the UnicastBusConfig section in your configuration file. " + "It may also be the case that the given queue just hasn't been created yet, or has been deleted.", ex.Queue, messageDescription), ex);
            }
        }

        public void NativeSendOrDefer(string destination,DeliveryMessageOptions deliveryMessageOptions, OutgoingMessage message)
        {
            SetTransportHeaders(deliveryMessageOptions.TimeToBeReceived, deliveryMessageOptions.NonDurable, message);

            try
            {
                SendOrDefer(destination,message, deliveryMessageOptions as SendMessageOptions);
            }
            catch (QueueNotFoundException ex)
            {
                var messageDescription = "ControlMessage";

                string enclosedMessageTypes;

                if (message.Headers.TryGetValue(Headers.EnclosedMessageTypes, out enclosedMessageTypes))
                {
                    messageDescription = enclosedMessageTypes;
                }
                throw new Exception(string.Format("The destination queue '{0}' could not be found. You may have mis-configured the destination for this kind of message ({1}) in the MessageEndpointMappings of the UnicastBusConfig section in your configuration file. " + "It may also be the case that the given queue just hasn't been created yet, or has been deleted.", ex.Queue, messageDescription), ex);
            }
        }

        void SetTransportHeaders(TimeSpan? timeToBeReceived, bool? nonDurable, OutgoingMessage message)
        {
            message.Headers[Headers.MessageId] = message.MessageId;


            if (timeToBeReceived.HasValue)
            {
                message.Headers[Headers.TimeToBeReceived] = timeToBeReceived.Value.ToString("c");
            }

            if (nonDurable.HasValue && nonDurable.Value)
            {
                message.Headers[Headers.NonDurableMessage] = true.ToString();
            }
        }

        void SendOrDefer(string destination,OutgoingMessage message, SendMessageOptions options)
        {
            if ((options.DelayDeliveryFor.HasValue && options.DelayDeliveryFor > TimeSpan.Zero) ||
                (options.DeliverAt.HasValue && options.DeliverAt.Value.ToUniversalTime() > DateTime.UtcNow))
            {
                SetIsDeferredHeader(message.Headers);
                MessageDeferral.Defer(message, new TransportDeferOptions(
                    destination,
                    options.DelayDeliveryFor,
                    options.DeliverAt,
                    options.NonDurable ?? true,
                    options.EnlistInReceiveTransaction));

                return;
            }

            MessageSender.Send(message, new TransportSendOptions(destination,
                                                                    options.TimeToBeReceived,
                                                                    options.NonDurable ?? true,
                                                                    options.EnlistInReceiveTransaction));
        }

        static void SetIsDeferredHeader(Dictionary<string, string> headers)
        {
            headers[Headers.IsDeferredMessage] = true.ToString();
        }

        void Publish(OutgoingMessage message, TransportPublishOptions publishOptions)
        {
            if (MessagePublisher == null)
            {
                throw new InvalidOperationException("No message publisher has been registered. If you're using a transport without native support for pub/sub please enable the message driven publishing feature by calling config.EnableFeature<MessageDrivenSubscriptions>() in your configuration");
            }
            MessagePublisher.Publish(message, publishOptions);
        }

        public class State
        {
            public State()
            {
                Headers = new Dictionary<string, string>();
                MessageId = CombGuid.Generate().ToString();
            }
            public Dictionary<string, string> Headers { get; private set; }
            public string MessageId { get; set; }
        }

    }
}