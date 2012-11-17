using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Data.Models
{
    public class Setting : Model
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual string Type { get; set; }
        public virtual SettingPermissions Permissions { get; set; }
    }

    [Flags]
    public enum SettingPermissions
    {
        None = 0,
        Read = 1,
        Write = 2
    }
}
