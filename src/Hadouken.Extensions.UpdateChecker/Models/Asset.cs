using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Hadouken.Extensions.UpdateChecker.Models
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Asset
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "browser_download_url")]
        public Uri BrowserDownloadUrl { get; set; }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        private string DebuggerDisplay
        {
            get { return string.Format("{0}", Name); }
        }
    }
}
