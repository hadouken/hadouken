namespace Hadouken.JsonRpc.Dto
{
    public sealed class PluginListItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string State { get; set; }

        public string UpdateVersion { get; set; }
    }
}
