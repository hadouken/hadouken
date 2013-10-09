using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Configuration
{
    public class HttpAuthConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("userName", IsRequired = true)]
        public string UserName
        {
            get { return this["userName"].ToString(); }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return this["password"].ToString(); }
            set { this["password"] = value; }
        }
    }
}
