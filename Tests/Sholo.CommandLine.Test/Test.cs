using System;
using System.Threading.Tasks;
using Xunit;

namespace Sholo.CommandLine.Test
{
    public class Test
    {
        [Fact]
        public async Task CommandBuilder_Works()
        {
            var app = new CommandLineAppBuilder()
                .WithCommand<TestCommand>(
                    "a",
                    "b",
                    bld =>
                    {
                    })
                .Build();

            await app.ExecuteAsync(Array.Empty<string>());
        }
    }
}
