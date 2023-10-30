using CommandLine;

namespace Sholo.CommandLine.Containers;

public class ContainerizedAppOptions
{
    [Option('c', "configfile", Required = false, HelpText = "The config file to load (.yaml or .json extension)")]
    public string ConfigFile { get; set; }
}
