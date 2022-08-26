using System;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.Services.Host
{
    public interface IHostServiceProviderFactoryBuilder : IServiceProviderFactoryBuilder<IHostServicesContext>
    {
        IServiceProvider BuildHostServiceProvider();
        IServiceProvider BuildCommandPluginServiceProvider(IServiceProviderFactoryBuilder[] builders);

        void SetServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory);
    }
}
