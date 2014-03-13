using System.Collections.Generic;
using Hadouken.Http.Api.Models;

namespace Hadouken.Http.Api
{
    public interface IReleasesRepository
    {
        IEnumerable<ReleaseItem> GetAll();
    }
}