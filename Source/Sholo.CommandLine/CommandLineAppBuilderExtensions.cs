using Sholo.Utils.Logging;

namespace Sholo.CommandLine;

public static class CommandLineAppBuilderExtensions
{
    public static ICommandLineAppBuilder<TSelf> UseSerilogConsole<TSelf>(this ICommandLineAppBuilder<TSelf> builder)
        where TSelf : ICommandLineAppBuilder<TSelf>
    {
        builder.ConfigureLogging((ctx, lb) => lb.AddSerilogConsole(ctx.HostConfiguration));
        return builder;
    }
}
