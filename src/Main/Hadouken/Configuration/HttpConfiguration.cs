using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Configuration
{
    public sealed class HttpConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("hostBinding", IsRequired = true)]
        public string HostBinding
        {
            get { return this["hostBinding"].ToString(); }
            set { this["hostBinding"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return Convert.ToInt32(this["port"]); }
            set { this["port"] = value; }
        }
    }
}
