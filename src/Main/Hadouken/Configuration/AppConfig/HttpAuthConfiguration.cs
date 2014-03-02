using System.Configuration;

namespace Hadouken.Configuration.AppConfig
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
