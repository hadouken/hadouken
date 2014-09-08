using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hadouken.Common.Reflection
{
    public sealed class AssemblyNameFinder : IAssemblyNameFinder
    {
        public IEnumerable<AssemblyName> GetAssemblyNames<T>(IEnumerable<string> filenames)
        {
            var assemblyCheckerType = typeof(AssemblyChecker);
            var temporaryDomain = CreateTemporaryAppDomain();
            try
            {
                var checker = (AssemblyChecker)temporaryDomain.CreateInstanceAndUnwrap(
                    assemblyCheckerType.Assembly.FullName,
                    assemblyCheckerType.FullName ?? string.Empty);

                return checker.GetAssemblyNames(filenames.ToArray(), typeof(T));
            }
            finally
            {
                AppDomain.Unload(temporaryDomain);
            }
        }

        /// <summary>
        /// Creates a temporary app domain.
        /// </summary>
        /// <returns>The created app domain.</returns>
        private static AppDomain CreateTemporaryAppDomain()
        {
            return AppDomain.CreateDomain(
                "ModuleLoader",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);
        }

        /// <summary>
        /// This class is loaded into the temporary appdomain to load and check if the assemblies match the filter.
        /// </summary>
        private class AssemblyChecker : MarshalByRefObject
        {
            public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames, Type type)
            {
                var result = new List<AssemblyName>();
                foreach (var filename in filenames)
                {
                    Assembly assembly;
                    if (File.Exists(filename))
                    {
                        try
                        {
                            assembly = Assembly.LoadFrom(filename);
                        }
                        catch (BadImageFormatException)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        try
                        {
                            assembly = Assembly.Load(filename);
                        }
                        catch (FileNotFoundException)
                        {
                            continue;
                        }
                    }

                    if (assembly.GetTypes().Any(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t)))
                    {
                        result.Add(assembly.GetName(false));
                    }
                }

                return result;
            }
        }
    }
}
