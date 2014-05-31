using System.Linq;
using Hadouken.Plugins;
using Nancy;

namespace Hadouken.Http.Management.Modules
{
    public sealed class ApiModule : NancyModule
    {
        public ApiModule(IPluginEngine pluginEngine)
        {
            Get["/api/scripts"] = _ => (from pluginManager in pluginEngine.GetAll()
                where pluginManager.State == PluginState.Loaded
                let manifest = pluginManager.Manifest
                where manifest.UserInterface != null
                where manifest.UserInterface.BackgroundScripts.Any()
                from backgroundScript in manifest.UserInterface.BackgroundScripts
                select string.Concat("plugins/", pluginManager.Package.Id, "/", backgroundScript)).ToArray();
        }
    }
}
