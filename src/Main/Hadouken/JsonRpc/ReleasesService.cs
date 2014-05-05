using System.Collections.Generic;
using Hadouken.Fx.JsonRpc;
using Hadouken.Http.Api;

namespace Hadouken.JsonRpc
{
    public sealed class ReleasesService : IJsonRpcService
    {
        private readonly IReleasesRepository _releasesRepository;

        public ReleasesService(IReleasesRepository releasesRepository)
        {
            _releasesRepository = releasesRepository;
        }

        [JsonRpcMethod("core.releases.list")]
        public IEnumerable<Http.Api.Models.ReleaseItem> List()
        {
            return _releasesRepository.GetAll();
        }
    }
}
