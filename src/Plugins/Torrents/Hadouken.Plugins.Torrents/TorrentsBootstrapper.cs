using System;

using Hadouken.Framework;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            return new TorrentsPlugin();
        }
    }
}
