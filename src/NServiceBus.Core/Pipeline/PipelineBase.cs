﻿namespace NServiceBus.Pipeline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Janitor;
    using NServiceBus.Settings;
    using NServiceBus.Unicast.Transport;
    using ObjectBuilder;

    [SkipWeaving]
    class PipelineBase<T>
        where T : BehaviorContext
    {
        public PipelineBase(IBuilder builder, ReadOnlySettings settings, PipelineModifications pipelineModifications, RegisterStep receiveBehavior = null)
        {
            busNotifications = builder.Build<BusNotifications>();
            contextStacker = builder.Build<BehaviorContextStacker>();

            var coordinator = new StepRegistrationsCoordinator(pipelineModifications.Removals, pipelineModifications.Replacements);
            if (receiveBehavior != null)
            {
                coordinator.Register(receiveBehavior);
            }
            foreach (var rego in pipelineModifications.Additions.Where(x => x.IsEnabled(settings)))
            {
                coordinator.Register(rego);
            }

            steps = coordinator.BuildPipelineModelFor<T>();

            behaviors = steps.Select(r => r.CreateBehavior(builder)).ToArray();
        }

        public void Initialize(PipelineInfo pipelineInfo)
        {
            foreach (var behaviorInstance in behaviors)
            {
                behaviorInstance.Initialize(pipelineInfo);
            }
        }

        public async Task Warmup()
        {
            foreach (var result in behaviors.Select(x => x.Warmup()))
            {
                await result;
            }
        }

        public async Task Cooldown()
        {
            foreach (var result in behaviors.Select(x => x.Cooldown()))
            {
                await result;
            }
        }

        public void Invoke(T context)
        {
            var lookupSteps = steps.ToDictionary(rs => rs.BehaviorType, ss => ss.StepId);
            var pipeline = new BehaviorChain(behaviors, context, lookupSteps, busNotifications);
            pipeline.Invoke(contextStacker);
        }

        BehaviorInstance[] behaviors;
        BusNotifications busNotifications;
        BehaviorContextStacker contextStacker;
        IList<RegisterStep> steps;
    }
}
