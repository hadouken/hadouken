using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Data;
using Hadouken.Configuration;

namespace Hadouken.Data.Models
{
    public class Setting : Model
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual string Type { get; set; }
        public virtual Permissions Permissions { get; set; }
        public virtual Options Options { get; set; }
    }
}
