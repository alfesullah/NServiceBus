namespace NServiceBus.Pipeline
{
    using System;

    /// <summary>
    /// Marks the inner most behavior of the pipeline
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PipelineTerminator<T> : StageConnector<T, PipelineTerminator<T>.TerminatingContext>, IPipelineTerminator where T : BehaviorContext
    {
       
        /// <summary>
        /// This method will be the final one to be called before the pipeline starts to travers back up the "stack"
        /// </summary>
        /// <param name="context"></param>
        public abstract void Terminate(T context);


        /// <summary>
        /// Invokes the terminate method
        /// </summary>
        /// <param name="context">Context object</param>
        /// <param name="next">Ignored since there by definition is no next behavior to call</param>
        public override void Invoke(T context, Action<TerminatingContext> next)
        {
            Terminate(context);
        }

        /// <summary>
        /// A wellknow context that terminates the pipeline
        /// </summary>
        public class TerminatingContext : BehaviorContext
        {
            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="parentContext"></param>
            public TerminatingContext(BehaviorContext parentContext)
                : base(parentContext)
            {

            }
        }

    }

    /// <summary>
    /// Marker interface for pipeline terminators
    /// </summary>
    public interface IPipelineTerminator
    {
    }
}