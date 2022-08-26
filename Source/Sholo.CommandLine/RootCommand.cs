using System.Threading;
using System.Threading.Tasks;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine
{
    public class RootCommand : ICommand
    {
        public Task<int> RunAsync(ICommandContext context, CancellationToken cancellationToken)
        {
            context.Command.ShowHelp();
            return Task.FromResult(1);
        }
    }
}
