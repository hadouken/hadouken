using System.Collections.Generic;

namespace Hadouken.Core.Services.Models {
    public sealed class NotifierConfiguration {
        public NotifierConfiguration() {
            this.Properties = new List<ConfigurationItem>();
        }

        public string Key { get; set; }
        public IList<ConfigurationItem> Properties { get; set; }
    }
}