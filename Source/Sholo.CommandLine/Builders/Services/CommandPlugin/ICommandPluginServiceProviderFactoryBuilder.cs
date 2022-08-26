using System;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.Services.CommandPlugin
{
    public interface ICommandPluginServiceProviderFactoryBuilder : IServiceProviderFactoryBuilder<ICommandServicesContext>
    {
        void SetInheritanceMode(ServiceInheritanceMode inheritanceMode);

        IServiceProvider BuildCommandPluginServiceProvider();
    }

    public interface ICommandPluginServiceProviderFactoryBuilder<TParameters> : IServiceProviderFactoryBuilder<ICommandServicesContext<TParameters>>
        where TParameters : class, new()
    {
        void SetInheritanceMode(ServiceInheritanceMode inheritanceMode);

        IServiceProvider BuildCommandPluginServiceProvider();
    }
}
