using System.IO;
using System.Linq;
using System.Reflection;

namespace Hadouken.Common.Extensibility {
    public static class ExtensionExtensions {
        public static string GetId(this IExtension extension) {
            return extension.GetType().GetCustomAttribute<ExtensionAttribute>().ExtensionId;
        }

        public static string[] GetScripts(this IExtension extension) {
            var attr = extension.GetType().GetCustomAttribute<ExtensionAttribute>();
            return attr.Scripts ?? Enumerable.Empty<string>().ToArray();
        }

        public static byte[] GetResource(this IExtension extension, string path) {
            var attr = extension.GetType().GetCustomAttribute<ExtensionAttribute>();
            var ns = attr.ResourceNamespace;
            var asm = extension.GetType().Assembly;

            path = path.Replace("/", ".");

            using (var ms = new MemoryStream()) {
                using (var resourceStream = asm.GetManifestResourceStream(string.Concat(ns, ".", path))) {
                    if (resourceStream == null) {
                        return null;
                    }

                    resourceStream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}