using Hadouken.Common.Data;

namespace HdknPlugins.Rss.Data.Models
{
    public class Filter : Model
    {
        public virtual Feed Feed { get; set; }
        public virtual string Label { get; set; }
        public virtual bool AutoStart { get; set; }
        public virtual string IncludeFilter { get; set; }
        public virtual string ExcludeFilter { get; set; }
    }
}
