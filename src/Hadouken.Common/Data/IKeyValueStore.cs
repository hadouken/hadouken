namespace Hadouken.Common.Data
{
    public interface IKeyValueStore
    {
        T Get<T>(string key, T defaultValue = default(T));

        void Set<T>(string key, T value);
    }
}
