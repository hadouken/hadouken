namespace Hadouken.Common.Extensibility {
    public interface IPlugin : IExtension {
        void Load();
        void Unload();
    }
}