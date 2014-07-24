using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                TypeName = obj.GetType().FullName;
                Value = obj;
            }

            public string TypeName { get; set; }

            public object Value { get; set; }
        }

        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ConcurrentDictionary<string, object> _store;
        private readonly object _loadLock = new object();
        private bool _loaded;

        public KeyValueStore(IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (jsonSerializer == null) throw new ArgumentNullException("jsonSerializer");

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

        }

        private void Load()
        {
            
        }
    }
}