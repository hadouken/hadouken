using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Config.Data
{
    public class JsonDataStore : IConfigDataStore
    {
        private readonly string _dataPath;
        private readonly Lazy<JObject> _jsonDictionary;

        public JsonDataStore(string dataPath)
        {
            _dataPath = dataPath;
            _jsonDictionary = new Lazy<JObject>(Load);
        }

        private JObject Load()
        {
            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath);

            var jsonFile = Path.Combine(_dataPath, "data.json");

            if (!File.Exists(jsonFile))
                return new JObject();

            var data = File.ReadAllText(jsonFile);
            return JObject.Parse(data);
        }

        protected JObject JsonDictionary
        {
            get { return _jsonDictionary.Value; }
        }

        public object Get(string key)
        {
            return JsonDictionary[key];
        }

        public IDictionary<string, object> GetStartingWith(string section)
        {
            return JsonDictionary.Properties()
                .Where(p => p.Name.StartsWith(section))
                .ToDictionary(p => p.Name, p => p.Value as object);
        }

        public void Set(string key, object value)
        {
            JsonDictionary[key] = JToken.FromObject(value);
        }

        public void Delete(string key)
        {
            JsonDictionary.Remove(key);
        }

        public void Save()
        {
            var jsonFile = Path.Combine(_dataPath, "data.json");
            var content = JsonConvert.SerializeObject(JsonDictionary);

            File.WriteAllText(jsonFile, content, Encoding.UTF8);
        }
    }
}
