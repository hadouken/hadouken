namespace Hadouken.Core.Services.Models {
    public sealed class ConfigurationItem {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
    }
}