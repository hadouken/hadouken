using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Dispatcher;
using System.Reflection;

namespace Hadouken.Common.Http.HttpListener
{
    internal class LocalizedAssembliesResolver : IAssembliesResolver
    {
        private readonly Assembly[] _assemblies;

        public LocalizedAssembliesResolver(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public ICollection<Assembly> GetAssemblies()
        {
            return _assemblies;
        }
    }
}
