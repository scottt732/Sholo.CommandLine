using System.ComponentModel.DataAnnotations;

namespace Sholo.CommandLine.Containers;

public class ApplicationNameAndVersion
{
    [Required]
    public string Name { get; set; }

    public string Version { get; set; }
}
