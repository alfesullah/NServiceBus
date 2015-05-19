namespace NServiceBus
{
    using System.Collections.Generic;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Transports;

    class DispatchMessageToTransportTerminator : PipelineTerminator<PhysicalOutgoingContextStageBehavior.Context>
    {
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

            var message = new OutgoingMessage(state.MessageId, state.Headers, context.Body);

            context.Get<RoutingStrategy>()
                .Dispatch(message);
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