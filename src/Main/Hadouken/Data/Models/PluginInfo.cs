using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Data.Models
{
    public class PluginInfo : IModel
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Version Version { get; set; }
        public virtual string Path { get; set; }
        public virtual PluginState State { get; set; }
    }

    public enum PluginState : int
    {
        Uninstalled = 0,
        Installed,
        Deactivated,
        Activated
    }
}
