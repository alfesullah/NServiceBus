namespace NServiceBus.Pipeline
{
    using System;

    abstract class PipelineTerminator<T> : StageConnector<T, PipelineTerminator<T>.TerminatingContext> where T : BehaviorContext
    {
        public class TerminatingContext : BehaviorContext
        {
            public TerminatingContext(BehaviorContext parentContext)
                : base(parentContext)
            {

            }
        }

        public abstract void Terminate(T context);


        public override void Invoke(T context, Action<TerminatingContext> next)
        {
            Terminate(context);
        }
    }
}