using System.Collections.Generic;
using Hadouken.Extensions.UpdateChecker.Models;

namespace Hadouken.Extensions.UpdateChecker.Http {
    public interface IGitHubReleasesClient {
        IEnumerable<Release> ListReleases();
    }
}