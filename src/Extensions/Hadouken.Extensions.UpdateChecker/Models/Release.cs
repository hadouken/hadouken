using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Hadouken.Extensions.UpdateChecker.Models
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Release
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "tag_name")]
        public string TagName { get; set; }

        [DataMember(Name = "html_url")]
        public Uri HtmlUrl { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "assets")]
        public Asset[] Assets { get; set; }

        private string DebuggerDisplay
        {
            get { return string.Format("Id: {0}, Tag: {1}", Id, TagName); }
        }
    }
}
