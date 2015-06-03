using System.Collections.Generic;
using System.Reflection;

namespace Hadouken.Common.Reflection {
    public interface IAssemblyNameFinder {
        IEnumerable<AssemblyName> GetAssemblyNames<T>(IEnumerable<string> filenames);
    }
}