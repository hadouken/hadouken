using Hadouken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.Rss.Data.Models
{
    [Table("plugin_Rss_Filters")]
    public class Filter : Model
    {
        public virtual Feed Feed { get; set; }
        public virtual string Label { get; set; }
        public virtual bool AutoStart { get; set; }
        public virtual string IncludeFilter { get; set; }
        public virtual string ExcludeFilter { get; set; }
    }
}
