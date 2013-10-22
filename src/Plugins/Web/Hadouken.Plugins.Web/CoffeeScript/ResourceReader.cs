using System.IO;

namespace Hadouken.Plugins.Web.CoffeeScript
{
    internal static class ResourceReader
    {
        public static string Read(string resourceName)
        {
            using (var resource = typeof (ResourceReader).Assembly.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                    return null;
                
                using (var reader = new StreamReader(resource))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}