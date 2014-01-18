namespace Hadouken.Plugins
{
    public enum PluginState
    {
        Unknown = 0,
        Unloaded,
        Unloading,
        Loading,
        Loaded,
        Updating,
        Error
    }
}
