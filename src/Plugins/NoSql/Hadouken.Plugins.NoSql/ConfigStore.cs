using System;
using Newtonsoft.Json;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace Hadouken.Plugins.NoSql
{
    public class ConfigDto
    {
        public string Data { get; set; }
    }

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
                var dto = session.Load<ConfigDto>(key);

                if (dto == null || String.IsNullOrEmpty(dto.Data))
                    return null;

                return JsonConvert.DeserializeObject(dto.Data);
            }
        }

        public void Set(string key, object value)
        {
            using (var session = _documentStore.Value.OpenSession())
            {
                session.Store(new ConfigDto {Data = JsonConvert.SerializeObject(value)}, key);
                session.SaveChanges();
            }
        }

        public void Delete(string key)
        {
            using (var session = _documentStore.Value.OpenSession())
            {
            }
        }
    }
}
