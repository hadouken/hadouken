using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Hadouken.Common.IO;
using Hadouken.Common.Text;

namespace Hadouken.Common.Data
{
    public class KeyValueStore : IKeyValueStore, IDisposable
    {
        private class ValueWrapper
        {
            public ValueWrapper()
            {
            }

            public ValueWrapper(object obj)
            {
                if (obj == null) throw new ArgumentNullException("obj");
                TypeName = obj.GetType().AssemblyQualifiedName;
                Value = obj;
            }

            public string TypeName { get; set; }

            public object Value { get; set; }
        }

        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ConcurrentDictionary<string, object> _store;
        private readonly object _loadLock = new object();
        private bool _loaded;

        public KeyValueStore(IEnvironment environment, IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            if (environment == null) throw new ArgumentNullException("environment");
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (jsonSerializer == null) throw new ArgumentNullException("jsonSerializer");

            _environment = environment;
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
            _store = new ConcurrentDictionary<string, object>();
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            lock (_loadLock)
            {
                if (!_loaded)
                {
                    Load();
                    _loaded = true;
                }
            }

            object val;
            if (!_store.TryGetValue(key, out val)) return defaultValue;

            return (T)val;
        }

        public void Set<T>(string key, T value)
        {
            _store.AddOrUpdate(key, value, (s, o) => value);
        }

        public void Dispose()
        {
            var data = new Dictionary<string, ValueWrapper>();

            foreach (var key in _store.Keys)
            {
                object val;
                if (!_store.TryGetValue(key, out val)) continue;

                data.Add(key, new ValueWrapper(val));
            }

            // Save data to disk
            var json = _jsonSerializer.SerializeObject(data);
            var path = _environment.GetApplicationDataPath().CombineWithFilePath("data.json");

            using (var file = _fileSystem.GetFile(path).Open(FileMode.Create))
            using (var writer = new StreamWriter(file))
            {
                writer.Write(json);
            }
        }

        private void Load()
        {
            var dataPath = _environment.GetApplicationDataPath();
            if (!_fileSystem.Exist(dataPath)) _fileSystem.GetDirectory(dataPath).Create();

            var path = dataPath.CombineWithFilePath("data.json");
            var file = _fileSystem.GetFile(path);

            if (!file.Exists) return;

            using (var stream = file.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                var data = _jsonSerializer.DeserializeObject<IDictionary<string, ValueWrapper>>(json);

                foreach (var key in data.Keys)
                {
                    var wrapper = data[key];
                    var type = Type.GetType(wrapper.TypeName);

                    var tempJson = _jsonSerializer.SerializeObject(wrapper.Value);
                    var realValue = _jsonSerializer.DeserializeObject(tempJson, type);

                    _store.TryAdd(key, realValue);
                }
            }
        }
    }
}