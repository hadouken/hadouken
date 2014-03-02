using System;

namespace Hadouken.Fx.JsonRpc
{
    public interface IJsonSerializer
    {
        Type[] ArrayTypes { get; }

        object Deserialize(string json);

        object Deserialize(string json, Type targetType);

        T Deserialize<T>(string json);

        string Serialize(object obj);
    }
}
