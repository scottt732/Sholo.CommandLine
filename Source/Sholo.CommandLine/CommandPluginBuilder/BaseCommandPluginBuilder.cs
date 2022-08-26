using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.CommandPlugin;
using Sholo.CommandLine.Configuration;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.PluginConfiguration;

namespace Sholo.CommandLine.CommandPluginBuilder
{
    public abstract class BaseCommandPluginBuilder<TSelf> : ICoreCommandPluginBuilder<TSelf>
        where TSelf : BaseCommandPluginBuilder<TSelf>
    {
        protected string Description { get; set; }

        protected Collection<ITargetConfiguration<ICommandConfigurationContext, IConfigurationBuilder>> CommandConfigurationConfiguration { get; } = new Collection<ITargetConfiguration<ICommandConfigurationContext, IConfigurationBuilder>>();
        protected Collection<ITargetConfiguration<IPluginConfigurationContext, CommandLineApplication>> CommandConfiguration { get; } = new Collection<ITargetConfiguration<IPluginConfigurationContext, CommandLineApplication>>();

        private IDictionary<string, Func<IPluginConfigurationContext, ICommandPlugin>> CommandPlugins { get; } = new Dictionary<string, Func<IPluginConfigurationContext, ICommandPlugin>>(StringComparer.OrdinalIgnoreCase);

        protected BaseCommandPluginBuilder()
        {
            CommandConfiguration.Add(
                new TargetConfiguration<IPluginConfigurationContext, CommandLineApplication>(
                    (ctx, app) =>
                    {
                        foreach (var commandLinePlugin in CommandPlugins)
                        {
                            var commandName = commandLinePlugin.Key;
                            var pluginFactory = commandLinePlugin.Value;

                            app.Command(commandName, childCommand =>
                            {
                                var cmdContext = new PluginConfigurationContext(
                                    ctx.HostConfiguration,
                                    ctx.HostServices,
                                    ctx.LoggerFactory,
                                    ctx.HostServiceProviderFactoryBuilder,
                                    ctx.CommonCommandConfigurationConfigurator,
                                    ctx.CommonCommandServiceProviderFactoryBuilder,
                                    null); // TODO: <-- inheritance of commands

                                var plugin = pluginFactory.Invoke(cmdContext);

                                childCommand.Description = plugin.Description;

                                plugin.ConfigureCommand(ctx, childCommand);
                            });
                        }
                    }
                )
            );
        }

        public TSelf WithDescription(string description)
        {
            Description = description;
            return (TSelf)this;
        }

        public TSelf ConfigureCommand(Action<IPluginConfigurationContext, CommandLineApplication> command)
        {
            CommandConfiguration.Add(new TargetConfiguration<IPluginConfigurationContext, CommandLineApplication>(command));
            return (TSelf)this;
        }

        public TSelf WithCommand<TCommand>(
            string name,
            string description,
            Action<CommandPluginBuilder<TCommand>> configurator
        )
            where TCommand : ICommand
        {
            var cpb = new CommandPluginBuilder<TCommand>()
                .WithDescription(description);

            configurator.Invoke(cpb);

            CommandPlugins[name] = b => cpb.Build(b);
            return (TSelf)this;
        }

        public TSelf WithCommand<TCommand, TCommandParameters>(
            string name,
            string description,
            Action<CommandPluginBuilder<TCommand, TCommandParameters>> configurator
        )
            where TCommand : ICommand<TCommandParameters>
            where TCommandParameters : class, new()
        {
            var cpb = new CommandPluginBuilder<TCommand, TCommandParameters>()
                .WithDescription(description);

            configurator.Invoke(cpb);

            CommandPlugins[name] = b => cpb.Build(b);
            return (TSelf)this;
        }

        // public TSelf WithCommand<TCommandPlugin>()
        //     where TCommandPlugin : class, ICommandPlugin, new()
        // {
        //     var commandPlugin = new TCommandPlugin();
        //     CommandPlugins.Add(commandPlugin.CommandName, commandPlugin);
        //     return (TSelf)this;
        // }

        // public TSelf WithCommand(ICommandPlugin commandPlugin)
        // {
        //     CommandPlugins.Add(commandPlugin.CommandName, commandPlugin);
        //     return (TSelf)this;
        // }

        public TSelf ConfigureCommandConfiguration(Action<ICommandConfigurationContext, IConfigurationBuilder> configurationBuilder)
        {
            CommandConfigurationConfiguration.Add(new TargetConfiguration<ICommandConfigurationContext, IConfigurationBuilder>(configurationBuilder));
            return (TSelf)this;
        }

        public abstract ICommandPlugin Build(IPluginConfigurationContext pluginConfigurationContext);
    }
}
