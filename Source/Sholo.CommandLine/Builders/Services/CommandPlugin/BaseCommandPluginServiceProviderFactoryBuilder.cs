using System;
using System.Collections.Generic;
using System.Linq;
using Sholo.CommandLine.Services;
using Sholo.CommandLine.Services.Host;

namespace Sholo.CommandLine.Builders.Services.CommandPlugin
{
    public abstract class BaseCommandPluginServiceProviderFactoryBuilder<TContext> : BaseServiceProviderFactoryBuilder<TContext>
    {
        protected IHostServiceProviderFactoryBuilder HostServiceProviderFactoryBuilder { get; }
        protected ServiceInheritanceMode InheritanceMode { get; private set; } = ServiceInheritanceMode.DoNotInherit;

        public void SetInheritanceMode(ServiceInheritanceMode inheritanceMode)
        {
            InheritanceMode = inheritanceMode;
        }

        public IServiceProvider BuildCommandPluginServiceProvider() => HostServiceProviderFactoryBuilder.BuildCommandPluginServiceProvider(
            GetPluginsForServices()
                .ToArray());

        protected BaseCommandPluginServiceProviderFactoryBuilder(
            IHostServiceProviderFactoryBuilder hostServiceProviderFactoryBuilder,
            IServiceProviderFactoryBuilder parentServiceProviderFactoryBuilder
        )
            : base(parentServiceProviderFactoryBuilder)
        {
            HostServiceProviderFactoryBuilder = hostServiceProviderFactoryBuilder;
        }

        private IEnumerable<IServiceProviderFactoryBuilder> GetPluginsForServices()
        {
            if (InheritanceMode == ServiceInheritanceMode.DoNotInherit)
            {
                yield return this;
                yield break;
            }

            var serviceConfigurations = new Stack<IServiceProviderFactoryBuilder>();
            serviceConfigurations.Push(this);

            var parent = this.Parent;
            while (parent != null)
            {
                serviceConfigurations.Push(parent);

                if (InheritanceMode == ServiceInheritanceMode.InheritParent)
                {
                    break;
                }

                parent = parent.Parent;
            }

            while (serviceConfigurations.Count > 0)
            {
                yield return serviceConfigurations.Pop();
            }
        }
    }
}
