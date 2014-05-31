using NuGet;

namespace Hadouken.Plugins
{
    public interface IDevelopmentPluginInstaller
    {
        IPackage GetPackage();
    }
}