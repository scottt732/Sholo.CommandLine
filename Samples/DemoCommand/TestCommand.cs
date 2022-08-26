using System;
using System.Threading;
using System.Threading.Tasks;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;

namespace DemoCommand
{
    public class TestCommand : ICommand<CommandParameters>
    {
        public Task<int> RunAsync(ICommandContext<CommandParameters> context, CancellationToken cancellationToken)
        {
            Console.WriteLine("Hi");
            return Task.FromResult(1);
        }
    }
}
