using System;
using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context.Logging
{
    public interface ILoggingContext
    {
        IConfiguration Configuration { get; }
    }
}
