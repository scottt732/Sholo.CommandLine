using Sholo.CommandLine.Context;
using Sholo.CommandLine.Services;
using Sholo.CommandLine.Services.CommandPlugin;
using Sholo.CommandLine.Services.Host;

namespace Sholo.CommandLine.Builders.Services.CommandPlugin
{
    public sealed class CommandPluginServiceProviderFactoryBuilder : BaseCommandPluginServiceProviderFactoryBuilder<ICommandServicesContext>, ICommandPluginServiceProviderFactoryBuilder
    {
        public CommandPluginServiceProviderFactoryBuilder(
            IHostServiceProviderFactoryBuilder hostServiceProviderFactoryBuilder,
            IServiceProviderFactoryBuilder parentServiceProviderFactoryBuilder
        )
            : base(hostServiceProviderFactoryBuilder, parentServiceProviderFactoryBuilder)
        {
        }
    }

    public sealed class CommandPluginServiceProviderFactoryBuilder<TParameters> : BaseCommandPluginServiceProviderFactoryBuilder<ICommandServicesContext<TParameters>>, ICommandPluginServiceProviderFactoryBuilder<TParameters>
        where TParameters : class, new()
    {
        public CommandPluginServiceProviderFactoryBuilder(
            IHostServiceProviderFactoryBuilder hostServiceProviderFactoryBuilder
        )
            : base(hostServiceProviderFactoryBuilder, null)
        {
        }
    }

    public sealed class CommandPluginServiceProviderFactoryBuilder<TParameters, TParentParameters> : BaseCommandPluginServiceProviderFactoryBuilder<ICommandServicesContext<TParameters>>, ICommandPluginServiceProviderFactoryBuilder<TParameters>
        where TParentParameters : class, new()
        where TParameters : class, TParentParameters, new()
    {
        public CommandPluginServiceProviderFactoryBuilder(
            IHostServiceProviderFactoryBuilder hostServiceProviderFactoryBuilder,
            IServiceProviderFactoryBuilder<TParentParameters> parentServiceProviderFactoryBuilder
        )
            : base(hostServiceProviderFactoryBuilder, parentServiceProviderFactoryBuilder)
        {
        }
    }
}
