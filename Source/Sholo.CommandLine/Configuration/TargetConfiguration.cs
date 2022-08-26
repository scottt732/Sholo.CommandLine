using System;

namespace Sholo.CommandLine.Configuration
{
    public class TargetConfiguration<TServicesContext, TTarget> : ITargetConfiguration<TServicesContext, TTarget>
    {
        public Type TargetType { get; } = typeof(TTarget);

        private Action<TServicesContext, TTarget> Factory { get; }

        public TargetConfiguration(Action<TServicesContext, TTarget> factory)
        {
            Factory = factory;
        }

        public void Configure(TServicesContext context, TTarget configure)
        {
            Factory?.Invoke(context, configure);
        }

        public void ConfigureTarget<TTargetType>(TServicesContext context, TTargetType target)
        {
            if (typeof(TTargetType) != TargetType)
            {
                throw new InvalidOperationException($"The type specified '{typeof(TTargetType).Name}' does not match the expected type '{typeof(TTarget).Name}'");
            }

            Configure(context, (TTarget)(object)target);
        }
    }
}
