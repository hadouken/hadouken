using System.Collections.Generic;
using System.Reflection;

namespace Hadouken.Common.Reflection
{
    public interface IEmbeddedResourceFinder
    {
        IEnumerable<IEmbeddedResource> Get(Assembly assembly);

        IEnumerable<IEmbeddedResource> GetAll();
    }
}
