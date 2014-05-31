using System.IO;
using Nancy;

namespace Hadouken.Http.Management.Modules
{
    public sealed class IndexModule : NancyModule
    {
        private static readonly string ResourceName = "Hadouken.Http.Management.UI.index.html";
        private static readonly string ContentType = "text/html";

        public IndexModule()
        {
            var data = GetIndexHtml();
            Get["/"] = _ => Response.FromStream(() => new MemoryStream(data), ContentType);
        }

        private byte[] GetIndexHtml()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(ResourceName))
            {
                if (stream == null) return null;

                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
