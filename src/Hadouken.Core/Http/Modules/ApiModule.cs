using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Extensibility;
using Nancy;

namespace Hadouken.Core.Http.Modules
{
    public sealed class ApiModule : NancyModule
    {
        public ApiModule(IEnumerable<IExtension> extensions)
            : base("api")
        {
            Get["/extensions"] = _ => extensions.Select(e => e.GetId());
        }
    }
}
