using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hadouken.DI;
using Hadouken.Reflection;
using System.Reflection;

namespace Hadouken
{
    public static class Kernel
    {
        private static IDependencyResolver _resolver;

        public static void Bootstrap(string workingDirectory)
        {
            if(String.IsNullOrEmpty(workingDirectory))
                throw new ArgumentNullException("workingDirectory");

            foreach (var file in Directory.GetFiles(workingDirectory, "*.dll"))
            {
                Assembly.LoadFile(file);
            }
        }

        public static void SetResolver(IDependencyResolver resolver)
        {
            if(resolver == null)
                throw new ArgumentNullException("resolver");

            _resolver = resolver;
        }

        public static IDependencyResolver Resolver
        {
            get
            {
                if(_resolver == null)
                    throw new ArgumentNullException();

                return _resolver;
            }
        }
    }
}
