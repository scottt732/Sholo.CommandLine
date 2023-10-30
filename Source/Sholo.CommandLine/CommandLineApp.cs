using System;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Sholo.CommandLine;

internal class CommandLineApp : ICommandLineApp
{
    // https://github.com/natemcmaster/CommandLineUtils/issues/111#issuecomment-401276354
    private static readonly int ExitCodeOperationCanceled;
    private static readonly int ExitCodeUnhandledException;

    static CommandLineApp()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // values from http://www.febooti.com/products/automation-workshop/online-help/events/run-dos-cmd-command/exit-codes/
            ExitCodeOperationCanceled = unchecked((int)0xC000013A);
            ExitCodeUnhandledException = unchecked((int)0xE0434F4D);
        }
        else
        {
            // Match Process.ExitCode (cfr bash) which uses 128 + signo.
            ExitCodeOperationCanceled = 130; // SIGINT
            ExitCodeUnhandledException = 134; // SIGABRT
        }
    }

    private CommandLineApplication Application { get; }

    public CommandLineApp(CommandLineApplication application)
    {
        Application = application;
    }

    public async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        void CancelHandler(object o, ConsoleCancelEventArgs e)
        {
            // ReSharper disable AccessToDisposedClosure

            if (!cts.IsCancellationRequested)
            {
                cts.Cancel();
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }

            // ReSharper restore AccessToDisposedClosure
        }

        void UnloadingHandler(AssemblyLoadContext ctx)
        {
            // ReSharper disable AccessToDisposedClosure

            if (!cts.IsCancellationRequested)
            {
                cts.Cancel();
            }

            // ReSharper restore AccessToDisposedClosure
        }

        try
        {
            Console.CancelKeyPress += CancelHandler;
            AssemblyLoadContext.Default.Unloading += UnloadingHandler;

            var result = await Application.ExecuteAsync(args, cts.Token);

            return result;
        }
        catch (OperationCanceledException)
        {
            return ExitCodeOperationCanceled;
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Unhandled exception: {e}");
            return ExitCodeUnhandledException;
        }
        finally
        {
            AssemblyLoadContext.Default.Unloading -= UnloadingHandler;
            Console.CancelKeyPress -= CancelHandler;
            cts.Dispose();
        }
    }
}
