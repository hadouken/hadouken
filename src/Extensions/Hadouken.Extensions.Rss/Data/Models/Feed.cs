using System;

namespace Hadouken.Extensions.Rss.Data.Models {
    public sealed class Feed {
        public Feed() {
            this.LastUpdatedTime = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int PollInterval { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}