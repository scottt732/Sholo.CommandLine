using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.Services.Host
{
    public class HostServiceProviderFactoryBuilder : BaseServiceProviderFactoryBuilder<IHostServicesContext>, IHostServiceProviderFactoryBuilder
    {
        public Type ServiceProviderFactoryContainerBuilderType { get; private set; }

        private Func<IServiceProvider> HostServiceProviderFactory { get; set; }
        private Func<IServiceProviderFactoryBuilder[], IServiceProvider> CommandPluginServiceProviderFactory { get; set; }

        public HostServiceProviderFactoryBuilder()
            : base(null)
        {
            HostServiceProviderFactory = () =>
            {
                var services = new ServiceCollection();

                ConfigureServiceCollection(services);

                return services.BuildServiceProvider();
            };

            CommandPluginServiceProviderFactory = plugins =>
            {
                var services = new ServiceCollection();

                foreach (var plugin in plugins)
                {
                    plugin.ConfigureServiceCollection(services);
                }

                return services.BuildServiceProvider();
            };
        }

        public void SetServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            ServiceProviderFactoryContainerBuilderType = typeof(TContainerBuilder);

            HostServiceProviderFactory = () =>
            {
                var services = new ServiceCollection();

                ConfigureServiceCollection(services);

                IServiceProvider serviceProvider;
                if (ServiceProviderFactoryContainerBuilderType == null && ConfiguredContainerBuilderType == null)
                {
                    serviceProvider = services.BuildServiceProvider();
                }
                else if (ServiceProviderFactoryContainerBuilderType == null && ConfiguredContainerBuilderType != null)
                {
                    throw new InvalidOperationException(
                        $"The application was configured with a container of type {ConfiguredContainerBuilderType.Name} " +
                        $"but does not have a service provider factory for that type");
                }
                else if (ServiceProviderFactoryContainerBuilderType != null && ConfiguredContainerBuilderType != null && ServiceProviderFactoryContainerBuilderType != ConfiguredContainerBuilderType)
                {
                    throw new InvalidOperationException(
                        $"The application was configured with a container of type {ConfiguredContainerBuilderType.Name} " +
                        $"but a service provider factory for {ServiceProviderFactoryContainerBuilderType.Name}");
                }
                else
                {
                    var builder = factory.CreateBuilder(services);

                    ConfigureContainerBuilder(ServiceProviderFactoryContainerBuilderType, builder);

                    serviceProvider = factory.CreateServiceProvider(builder);
                }

                return serviceProvider;
            };

            CommandPluginServiceProviderFactory = plugins =>
            {
                var services = new ServiceCollection();

                var configuredContainerBuilderTypes = plugins.Select(x => x.ConfiguredContainerBuilderType).Distinct().ToArray();

                if (configuredContainerBuilderTypes.Length > 1)
                {
                    throw new InvalidOperationException(
                        $"The plugin hierarchy contains more than 1 container builder type " +
                        $"({string.Join(", ", configuredContainerBuilderTypes.Select(x => x.Name))}). " +
                        $"If a container builder is used, there must be a matching service provider factory"
                    );
                }

                foreach (var plugin in plugins)
                {
                    plugin.ConfigureServiceCollection(services);
                }

                var configuredContainerBuilderType = configuredContainerBuilderTypes.FirstOrDefault();

                IServiceProvider serviceProvider;
                if (ServiceProviderFactoryContainerBuilderType == null && configuredContainerBuilderType == null)
                {
                    serviceProvider = services.BuildServiceProvider();
                }
                else if (ServiceProviderFactoryContainerBuilderType == null && configuredContainerBuilderType != null)
                {
                    throw new InvalidOperationException(
                        $"The application was configured with a container of type {configuredContainerBuilderType.Name} " +
                        $"but does not have a service provider factory for that type");
                }
                else if (ServiceProviderFactoryContainerBuilderType != null && configuredContainerBuilderType != null && ServiceProviderFactoryContainerBuilderType != configuredContainerBuilderType)
                {
                    throw new InvalidOperationException(
                        $"The application was configured with a container of type {configuredContainerBuilderType.Name} " +
                        $"but a service provider factory for {ServiceProviderFactoryContainerBuilderType.Name}");
                }
                else
                {
                    var builder = factory.CreateBuilder(services);

                    foreach (var plugin in plugins)
                    {
                        plugin.ConfigureContainerBuilder(ServiceProviderFactoryContainerBuilderType, builder);
                    }

                    serviceProvider = factory.CreateServiceProvider(builder);
                }

                return serviceProvider;
            };
        }

        public IServiceProvider BuildHostServiceProvider()
        {
            if (Context == null)
            {
                throw new InvalidOperationException($"You must call {nameof(SetContext)} before calling {nameof(BuildHostServiceProvider)}");
            }

            return HostServiceProviderFactory.Invoke();
        }

        public IServiceProvider BuildCommandPluginServiceProvider(IServiceProviderFactoryBuilder[] builders)
        {
            if (Context == null)
            {
                throw new InvalidOperationException($"You must call {nameof(SetContext)} before calling {nameof(BuildHostServiceProvider)}");
            }

            return CommandPluginServiceProviderFactory.Invoke(builders);
        }
    }
}
