using System;

namespace Hadouken.JsonRpc.Dto
{
    public sealed class PackageDetailsItem
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public string[] Authors { get; set; }

        public Uri IconUrl { get; set; }

        public string Summary { get; set; }

        public Uri Homepage { get; set; }
    }
}
