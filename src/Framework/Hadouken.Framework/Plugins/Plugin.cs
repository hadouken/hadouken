namespace Hadouken.Framework.Plugins
{
    public abstract class Plugin
    {
        public abstract void OnStart();

        public virtual void OnStop() {}
    }
}
