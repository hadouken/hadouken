using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.JsonRpc;

namespace Hadouken.Extensions.Importer.Services
{
    public sealed class ImportService : IJsonRpcService
    {
        private readonly IList<IImporter> _importers; 

        public ImportService(IEnumerable<IImporter> importers)
        {
            _importers = new List<IImporter>(importers);
        }

        [JsonRpcMethod("importer.import")]
        public void Import(string importerName, string path)
        {
            var importer = _importers.Single(i => i.Name == importerName);
            importer.Import(path);
        }

        [JsonRpcMethod("importer.getAll")]
        public IEnumerable<string> GetAllImporters()
        {
            return _importers.Select(i => i.Name);
        }
    }
}
