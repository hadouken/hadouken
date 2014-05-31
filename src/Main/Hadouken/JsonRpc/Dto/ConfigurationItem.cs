namespace Hadouken.JsonRpc.Dto
{
    public sealed class ConfigurationItem
    {
        public string ApplicationDataPath { get; set; }
        public string InstanceName { get; set; }
        public string HostBinding { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PluginDirectory { get; set; }
        public string PluginUrlTemplate { get; set; }
        public string PluginRepositoryUrl { get; set; }
    }
}
