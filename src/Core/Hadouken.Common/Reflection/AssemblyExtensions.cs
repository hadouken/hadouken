using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hadouken.Common.Reflection {
    public static class AssemblyExtensions {
        public static IEnumerable<Type> GetTypesAssignableFrom<T>(this Assembly assembly) {
            return (from type in assembly.GetTypes()
                where typeof (T).IsAssignableFrom(type)
                select type);
        }
    }
}