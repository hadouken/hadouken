using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Data.Models
{
    public class Setting : IModel
    {
        public virtual int Id { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}
