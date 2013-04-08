using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common
{
    public static class Kernel
    {
        public static T Get<T>()
        {
            throw new NotImplementedException();
        }

        public static object Get(Type type)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<T> GetAll<T>()
        {
            throw new NotImplementedException();
        } 

        public static IEnumerable<object> GetAll(Type type)
        {
            throw new NotImplementedException();
        } 
    }
}
