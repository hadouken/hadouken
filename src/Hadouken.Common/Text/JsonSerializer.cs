using System;
using Newtonsoft.Json;

namespace Hadouken.Common.Text
{
    public class JsonSerializer : IJsonSerializer
    {
        public object DeserializeObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}