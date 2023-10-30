using System.Threading;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine;

public interface ICommandLineApp
{
    Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default);
}
