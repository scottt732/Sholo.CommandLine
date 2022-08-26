using System;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.Context.Logging;

namespace Sholo.CommandLine.Builders.Logging
{
    public interface ILoggingBuilderConfigurator
    {
        void AddLoggingBuilderConfiguration(Action<ILoggingContext, ILoggingBuilder> configure);

        void ConfigureLoggingBuilder(ILoggingContext context, ILoggingBuilder loggingBuilder);
    }
}
