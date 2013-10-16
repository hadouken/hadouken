using Hadouken.Framework;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.WebUI
{
    public class WebBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            return new WebPlugin();
        }
    }
}
