using System.Threading;
using System.Threading.Tasks;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.Commands;

public interface ICommand<in TParameters>
    where TParameters : class, new()
{
    Task<int> RunAsync(ICommandContext<TParameters> context, CancellationToken cancellationToken);
}

public interface ICommand
{
    Task<int> RunAsync(ICommandContext context, CancellationToken cancellationToken);
}
