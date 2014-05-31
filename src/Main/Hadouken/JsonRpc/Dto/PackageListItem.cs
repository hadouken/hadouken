using System;

namespace Hadouken.JsonRpc.Dto
{
    public sealed class PackageListItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Authors { get; set; }

        public Uri IconUrl { get; set; }

        public string Summary { get; set; }

        public string Version { get; set; }
    }
}
