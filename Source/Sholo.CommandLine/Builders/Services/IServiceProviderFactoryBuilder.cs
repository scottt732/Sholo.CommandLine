using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.CommandLine.Services
{
    public interface IServiceProviderFactoryBuilder
    {
        Type ConfiguredContainerBuilderType { get; }

        IServiceProviderFactoryBuilder Parent { get; }

        void ConfigureServiceCollection(IServiceCollection serviceCollection);
        void ConfigureContainerBuilder(Type containerBuilderType, object containerBuilder);
    }

    public interface IServiceProviderFactoryBuilder<TContext> : IServiceProviderFactoryBuilder
    {
        void SetContext(TContext context);

        void AddServicesConfiguration(Action<TContext, IServiceCollection> servicesConfiguration);
        void AddContainerConfiguration<TContainerBuilder>(Action<TContext, TContainerBuilder> containerConfiguration);
    }
}
