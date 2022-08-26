using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.Configuration;
using Sholo.CommandLine.Context.Logging;

namespace Sholo.CommandLine.Builders.Logging
{
    public class LoggingBuilderConfigurator : ILoggingBuilderConfigurator
    {
        private IList<ITargetConfiguration<ILoggingContext, ILoggingBuilder>> LoggingBuilderConfigurations { get; } = new List<ITargetConfiguration<ILoggingContext, ILoggingBuilder>>();

        public void AddLoggingBuilderConfiguration(Action<ILoggingContext, ILoggingBuilder> configure)
        {
            LoggingBuilderConfigurations.Add(new TargetConfiguration<ILoggingContext, ILoggingBuilder>(configure));
        }

        public void ConfigureLoggingBuilder(ILoggingContext context, ILoggingBuilder loggingBuilder)
        {
            foreach (var cfg in this.LoggingBuilderConfigurations)
            {
                cfg.Configure(context, loggingBuilder);
            }
        }
    }
}
