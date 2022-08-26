using System;
using System.Threading.Tasks;
using Sholo.CommandLine;

namespace DemoCommand
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Console.WriteLine("X");

            var app = new CommandLineAppBuilder()
                .WithDescription("Testing")
                .WithCommand<TestCommand, CommandParameters>(
                    "test",
                    "b",
                    cfg =>
                    {
                        cfg.WithCommand<TestCommand, CommandParameters>(
                            "test2",
                            "Nested test",
                            icfg =>
                            {
                                icfg.WithCommand<TestCommand, CommandParameters>(
                                    "test3",
                                    "Nested test2",
                                    c =>
                                    {
                                    }
                                );
                            }
                        );
                    })
                .Build();

            return await app.ExecuteAsync(args);
        }
    }
}
