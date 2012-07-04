using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Hadouken.Reflection
{
    public static class ReflectionExtensions
    {
        public static T GetAttribute<T>(this Assembly asm)
        {
            return (T)asm.GetCustomAttributes(typeof(T), false).Where(attr => attr.GetType() == typeof(T)).FirstOrDefault();
        }

        public static bool HasAttribute<T>(this MemberInfo mi)
        {
            return mi.GetCustomAttributes(typeof(T), true).Any();
        }

        public static T GetAttribute<T>(this MemberInfo mi)
        {
            return (T)mi.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }
    }
}
