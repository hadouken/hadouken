using System;

namespace Hadouken.Fx.JsonRpc
{
    public class JsonSerializer : IJsonSerializer
    {
        public Type[] ArrayTypes
        {
            get { return new[] {typeof (JsonArray)}; }
        }

        public object Deserialize(string json)
        {
            return SimpleJson.DeserializeObject(json);
        }

        public object Deserialize(string json, Type targetType)
        {
            return SimpleJson.DeserializeObject(json, targetType);
        }

        public T Deserialize<T>(string json)
        {
            return SimpleJson.DeserializeObject<T>(json);
        }

        public string Serialize(object obj)
        {
            return SimpleJson.SerializeObject(obj);
        }
    }
}