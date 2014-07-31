using System;

namespace Hadouken.Extensions.Rss.Config
{
    public sealed class Feed
    {
        public Feed()
        {
            LastUpdatedTime = DateTimeOffset.UtcNow;
        }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public int PollInterval { get; set; }

        public DateTimeOffset LastUpdatedTime { get; set; }

        public Filter[] Filters { get; set; }
    }
}
