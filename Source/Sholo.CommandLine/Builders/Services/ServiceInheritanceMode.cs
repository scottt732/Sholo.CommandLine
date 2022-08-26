using JetBrains.Annotations;

namespace Sholo.CommandLine.Services
{
    [PublicAPI]
    public enum ServiceInheritanceMode
    {
        DoNotInherit,
        InheritParent,
        InheritAllParents
    }
}
