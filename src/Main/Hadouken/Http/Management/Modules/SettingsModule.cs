using Hadouken.Configuration;
using Hadouken.Http.Management.Models;
using Nancy;
using Nancy.ModelBinding;

namespace Hadouken.Http.Management.Modules
{
    public class SettingsModule : NancyModule
    {
        public SettingsModule(IConfiguration configuration)
            : base("settings")
        {
            Get["/"] = _ =>
            {
                var dto = new SettingsItem
                {
                    DataPath = configuration.ApplicationDataPath,
                    HttpHostBinding = configuration.Http.HostBinding,
                    HttpPort = configuration.Http.Port,
                    InstanceName = configuration.InstanceName,
                    PluginsBaseDirectory = configuration.Plugins.BaseDirectory,
                    PluginsRepositoryUri = configuration.Plugins.RepositoryUri
                };

                return View["Index", dto];
            };

            Post["/"] = _ =>
            {
                var dto = this.Bind<SettingsItem>();

                configuration.ApplicationDataPath = dto.DataPath;
                configuration.Http.HostBinding = dto.HttpHostBinding;
                configuration.Http.Port = dto.HttpPort;
                configuration.InstanceName = dto.InstanceName;
                configuration.Plugins.BaseDirectory = dto.PluginsBaseDirectory;
                configuration.Plugins.RepositoryUri = dto.PluginsRepositoryUri;
                configuration.Save();

                return View["Index", dto];
            };
        }
    }
}
