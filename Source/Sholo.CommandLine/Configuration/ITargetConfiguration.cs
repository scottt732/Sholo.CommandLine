using System;

namespace Sholo.CommandLine.Configuration
{
    public interface ITargetConfiguration<in TContext>
    {
        Type TargetType { get; }

        void ConfigureTarget<TTarget>(TContext context, TTarget target);
    }

    public interface ITargetConfiguration<in TContext, in TTarget> : ITargetConfiguration<TContext>
    {
        void Configure(TContext context, TTarget configure);
    }
}
