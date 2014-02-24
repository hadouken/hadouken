namespace Hadouken.Http.Management.Models
{
    public class PluginListItem
    {
        public string Name { get; set; }

        public string StateMessage { get; set; }

        public string Version { get; set; }

        public bool HasUpgrade { get; set; }
    }
}
