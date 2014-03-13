using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hadouken.Http.Api.Models
{
    [DataContract]
    public sealed class Plugin
    {
        public Plugin()
        {
            Releases = new List<ReleaseItem>();
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "author")]
        public string Author { get; set; }

        [DataMember(Name = "homepage")]
        public Uri Homepage { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "releases")]
        public IEnumerable<ReleaseItem> Releases { get; set; }
    }
}
