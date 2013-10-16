using System;
using Nancy;

namespace Hadouken.Plugins.WebUI.Modules
{
    public class PluginsModule : NancyModule
    {
        public PluginsModule()
        {
            Get["/plugins/{id}/{path*}"] = _ => String.Format("Id: {0}, Path: {1}", _.id, _.path);
        }
    }
}
