using Hadouken.Framework.Plugins;

namespace Hadouken.Framework
{
    public abstract class Bootstrapper
    {
        public abstract Plugin Load(IBootConfig config);
    }
}
