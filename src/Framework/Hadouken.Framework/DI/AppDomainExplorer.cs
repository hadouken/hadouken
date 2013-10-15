using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.DI
{
    public static class AppDomainExplorer
    {
        public static Type[] TypesInheritedFrom<TType>()
        {
            return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from type in asm.GetTypes()
                where type.IsClass && !type.IsAbstract
                where typeof (TType).IsAssignableFrom(type)
                select type).ToArray();
        }
    }
}
