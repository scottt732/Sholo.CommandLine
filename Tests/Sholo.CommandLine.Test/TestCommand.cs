using System;
using System.Threading;
using System.Threading.Tasks;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.Test
{
    public class TestCommand : ICommand
    {
        public Task<int> RunAsync(ICommandContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
