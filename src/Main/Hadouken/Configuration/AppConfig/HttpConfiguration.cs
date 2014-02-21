using System;
using System.Configuration;

namespace Hadouken.Configuration.AppConfig
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

        [ConfigurationProperty("auth", IsRequired = true)]
        public HttpAuthConfiguration Authentication
        {
            get { return this["auth"] as HttpAuthConfiguration; }
        }
    }
}
