using System;
using System.Linq;
using Hadouken.Plugins.NoSql.Models;
using Newtonsoft.Json;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace Hadouken.Plugins.NoSql
{
    public class ConfigStore : IConfigStore
    {
        private readonly Lazy<DocumentStore> _documentStore;

        public ConfigStore(string dataPath)
        {
            _documentStore = new Lazy<DocumentStore>(() => InitializeDocumentStore(dataPath));
        }

        private static DocumentStore InitializeDocumentStore(string dataPath)
        {
            var embeddedStore = new EmbeddableDocumentStore
            {
                DataDirectory = dataPath,
                RunInMemory = (dataPath == ":in-memory:")
            };

            embeddedStore.Initialize();

            return embeddedStore;
        }

        public object Get(string key)
        {
            using (var session = _documentStore.Value.OpenSession())
            {
                var dto = session.Load<Config>(key);

                if (dto == null || String.IsNullOrEmpty(dto.Value))
                    return null;

                return JsonConvert.DeserializeObject(dto.Value);
            }
        }

        public void Set(string key, object value)
        {
            using (var session = _documentStore.Value.OpenSession())
            {
                var serializedValue = JsonConvert.SerializeObject(value);
                var dto = session.Load<Config>(key);

                if (dto == null)
                {
                    dto = new Config {Id = key, Value = serializedValue};
                    session.Store(dto);
                }
                else
                {
                    dto.Value = JsonConvert.SerializeObject(value);
                }

                session.SaveChanges();
            }
        }

        public void Delete(string key)
        {
            using (var session = _documentStore.Value.OpenSession())
            {
                var dto = session.Load<Config>("configs/" + key);

                if (dto == null) return;

                session.Delete(dto);
                session.SaveChanges();
            }
        }
    }
}
