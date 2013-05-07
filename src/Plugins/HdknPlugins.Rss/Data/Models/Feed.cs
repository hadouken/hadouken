using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Data;

namespace HdknPlugins.Rss.Data.Models
{
    [Table("plugin_Rss_Feeds")]
    public class Feed : Model
    {
        public Feed()
        {
            Filters = new List<Filter>();
        }

        public virtual string Name { get; set; }
        public virtual string Url { get; set; }
        public virtual int PollInterval { get; set; }
        public virtual DateTime? LastUpdateTime { get; set; }

        public virtual IList<Filter> Filters { get; set; } 
    }
}
