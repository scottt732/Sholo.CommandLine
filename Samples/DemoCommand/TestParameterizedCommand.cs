using System.Threading;
using System.Threading.Tasks;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;

namespace DemoCommand
{
    public class TestParameterizedCommand : ICommand<CommandParameters>
    {
        public Task<int> RunAsync(ICommandContext<CommandParameters> context, CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
