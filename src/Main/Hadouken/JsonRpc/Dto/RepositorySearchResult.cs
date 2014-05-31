using System.Collections.Generic;

namespace Hadouken.JsonRpc.Dto
{
    public sealed class RepositorySearchResult
    {
        public int TotalPages { get; set; }

        public IEnumerable<PackageListItem> Items { get; set; }
    }
}
