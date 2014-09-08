using System.Collections.Generic;
using System.IO;
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
            Get["/extensions"] = _ =>
                (from extension in extensions
                    from script in extension.GetScripts()
                    select string.Concat("api/extensions/", extension.GetId(), "/", script));

            Get["/extensions/{id}/{path*}"] = _ =>
            {
                string id = _.id;
                string path = _.path;

                var extension = extensions.SingleOrDefault(e => e.GetId() == id);
                if (extension == null) return HttpStatusCode.NotFound;

                var resource = extension.GetResource(path);
                if (resource == null) return HttpStatusCode.NotFound;

                return Response.FromStream(() => new MemoryStream(resource), MimeTypes.GetMimeType(path));
            };
        }
    }
}
