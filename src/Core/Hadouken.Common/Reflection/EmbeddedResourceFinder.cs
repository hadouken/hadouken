using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hadouken.Common.Reflection
{
    public class EmbeddedResourceFinder : IEmbeddedResourceFinder
    {
        public IEnumerable<IEmbeddedResource> Get(Assembly assembly)
        {
            return Get(new[] {assembly});
        }

        public IEnumerable<IEmbeddedResource> GetAll()
        {
            return Get(AppDomain.CurrentDomain.GetAssemblies());
        }

        private static IEnumerable<IEmbeddedResource> Get(IEnumerable<Assembly> assemblies)
        {
            return (from asm in assemblies
                where !asm.IsDynamic
                from resourceName in asm.GetManifestResourceNames()
                let lastDotIndex = resourceName.LastIndexOf(".", StringComparison.Ordinal)
                let dotIndex = resourceName.Substring(0, lastDotIndex).LastIndexOf(".", StringComparison.Ordinal)
                let name = resourceName.Substring(dotIndex + 1)
                orderby name ascending
                select new EmbeddedResource(asm, resourceName, name));
        }
    }
}