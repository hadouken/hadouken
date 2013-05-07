using Hadouken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.AutoAdd.Data.Models
{
    [Table("plugin_AutoAdd_WatchedFolders")]
    public class WatchedFolder : Model
    {
        public virtual string Path { get; set; }
        public virtual string Label { get; set; }
        public virtual string IncludeFilter { get; set; }
        public virtual string ExcludeFilter { get; set; }
        public virtual bool AutoStart { get; set; }
    }
}
