using Hadouken.Common.Extensibility;

namespace Hadouken.Extensions.Importer
{
    [Extension("plugin.importer",
        Name = "Importer",
        Description = "Imports torrents from other clients such as uTorrent and qBitTorrent.",
        ResourceNamespace = "Hadouken.Extensions.Importer.Resources",
        Scripts = new[]
        {
            "js/app.js",
            "js/controllers/settingsController.js"
        }
    )]
    public class ImporterPlugin : IPlugin
    {
        public void Load()
        {
        }

        public void Unload()
        {
        }
    }
}
