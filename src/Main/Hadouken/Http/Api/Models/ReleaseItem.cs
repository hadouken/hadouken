using System;
using System.Runtime.Serialization;

namespace Hadouken.Http.Api.Models
{
    [DataContract]
    public class ReleaseItem
    {
        [DataMember(Name = "downloadUri")]
        public Uri DownloadUri { get; set; }

        [DataMember(Name = "releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }
    }
}