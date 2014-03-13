using System;
using System.Runtime.Serialization;

namespace Hadouken.Http.Api.Models
{
    [DataContract]
    public class PluginListItem
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "homepage")]
        public Uri Homepage { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "latestRelease")]
        public ReleaseItem LatestRelease { get; set; }
    }
}