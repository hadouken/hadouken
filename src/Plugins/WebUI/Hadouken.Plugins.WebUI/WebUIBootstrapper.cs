using Hadouken.Framework;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.WebUI
{
    public class WebUIBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            return new WebUIPlugin();
        }
    }
}
