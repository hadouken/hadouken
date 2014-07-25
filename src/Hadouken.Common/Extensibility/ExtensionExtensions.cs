using System.Reflection;

namespace Hadouken.Common.Extensibility
{
    public static class ExtensionExtensions
    {
        public static string GetId(this IExtension extension)
        {
            return extension.GetType().GetCustomAttribute<ExtensionAttribute>().ExtensionId;
        }
    }
}
