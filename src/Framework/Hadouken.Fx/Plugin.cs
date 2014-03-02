namespace Hadouken.Fx
{
    public abstract class Plugin
    {
        public abstract void Load();

        public virtual void Unload() { }
    }
}