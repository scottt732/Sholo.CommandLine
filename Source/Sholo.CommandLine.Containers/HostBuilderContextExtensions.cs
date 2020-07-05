using Microsoft.Extensions.Hosting;

namespace Sholo.CommandLine.Containers
{
    public static class HostBuilderContextExtensions
    {
        public static bool TryGetOptions<TOptions>(this HostBuilderContext context, out TOptions options)
            where TOptions : ContainerizedAppOptions
        {
            if (context.Properties.TryGetValue(HostBuilderExtensions.ContextPropertyName, out var optionsObject))
            {
                if (optionsObject is TOptions optionsResult)
                {
                    options = optionsResult;
                    return true;
                }
            }

            options = default;
            return false;
        }
    }
}
