using Hadouken.Common.Data;

namespace HdknPlugins.AutoAdd.Data.Models
{
    public class Folder : Model
    {
        public virtual string Path { get; set; }
        public virtual string Label { get; set; }
        public virtual string IncludeFilter { get; set; }
        public virtual string ExcludeFilter { get; set; }
        public virtual bool AutoStart { get; set; }
    }
}
