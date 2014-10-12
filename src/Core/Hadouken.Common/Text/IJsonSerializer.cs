using System;

namespace Hadouken.Common.Text
{
    public interface IJsonSerializer
    {
        object DeserializeObject(string json, Type type);

        T DeserializeObject<T>(string json);

        string SerializeObject(object obj);
    }
}
