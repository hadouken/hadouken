namespace Hadouken.Common.Extensibility
{
    public interface IPlugin
    {
        void Load();

        void Unload();
    }
}
