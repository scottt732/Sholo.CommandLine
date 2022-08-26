using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Configuration;

namespace Sholo.CommandLine.Services
{
    public abstract class BaseServiceProviderFactoryBuilder<TContext> : IServiceProviderFactoryBuilder<TContext>
    {
        public IServiceProviderFactoryBuilder Parent { get; }
        public Type ConfiguredContainerBuilderType { get; private set; }

        protected TContext Context { get; private set; }

        private IList<ITargetConfiguration<TContext, IServiceCollection>> ServicesConfiguration { get; } = new List<ITargetConfiguration<TContext, IServiceCollection>>();
        private IList<ITargetConfiguration<TContext>> ContainerConfiguration { get; } = new List<ITargetConfiguration<TContext>>();

        public void SetContext(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddServicesConfiguration(Action<TContext, IServiceCollection> servicesConfiguration)
        {
            ServicesConfiguration.Add(new TargetConfiguration<TContext, IServiceCollection>(servicesConfiguration));
        }

        public void AddContainerConfiguration<TContainerBuilder>(Action<TContext, TContainerBuilder> containerConfiguration)
        {
            if (ConfiguredContainerBuilderType == null)
            {
                ConfiguredContainerBuilderType = typeof(TContainerBuilder);
            }
            else if (typeof(TContainerBuilder) != ConfiguredContainerBuilderType)
            {
                throw new ArgumentException($"{nameof(AddContainerConfiguration)} has already been called for a container of type {ConfiguredContainerBuilderType.Name}. Cannot use {typeof(TContainerBuilder)}.");
            }

            ContainerConfiguration.Add(new TargetConfiguration<TContext, TContainerBuilder>(containerConfiguration));
        }

        public void ConfigureServiceCollection(IServiceCollection serviceCollection)
        {
            foreach (var x in this.ServicesConfiguration)
            {
                x.Configure(Context, serviceCollection);
            }
        }

        public void ConfigureContainerBuilder(Type containerBuilderType, object containerBuilder)
        {
            var methodInfo = GetType().GetMethod(nameof(ConfigureContainerBuilderImpl));
            if (methodInfo == null) throw new InvalidOperationException();

            var genericMethod = methodInfo.MakeGenericMethod(containerBuilderType);

            genericMethod.Invoke(this, new[] { containerBuilder });
        }

        protected BaseServiceProviderFactoryBuilder(IServiceProviderFactoryBuilder parent)
        {
            Parent = parent;
        }

        private void ConfigureContainerBuilderImpl<TTarget>(TTarget containerBuilder)
        {
            foreach (var x in this.ContainerConfiguration)
            {
                x.ConfigureTarget(Context, containerBuilder);
            }
        }
    }
}
