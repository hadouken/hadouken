using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Data.Models
{
    public class PluginInfo : IModel
    {
        public virtual int Id { get; set; }
        public virtual string Path { get; set; }
    }
}
