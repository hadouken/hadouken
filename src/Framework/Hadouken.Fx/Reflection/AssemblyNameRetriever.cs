using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hadouken.Fx.Reflection
{
    public class AssemblyNameRetriever
    {
        /// <summary>
        /// Gets all assembly names of the assemblies in the given files that match the filter.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>All assembly names of the assemblies in the given files that match the filter.</returns>
        public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames)
        {
            var assemblyCheckerType = typeof(AssemblyChecker);
            var temporaryDomain = CreateTemporaryAppDomain();
            try
            {
                var checker = (AssemblyChecker)temporaryDomain.CreateInstanceAndUnwrap(
                    assemblyCheckerType.Assembly.FullName,
                    assemblyCheckerType.FullName ?? string.Empty);

                return checker.GetAssemblyNames(filenames.ToArray());
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
                "NinjectModuleLoader",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);
        }

        /// <summary>
        /// This class is loaded into the temporary appdomain to load and check if the assemblies match the filter.
        /// </summary>
        private class AssemblyChecker : MarshalByRefObject
        {
            /// <summary>
            /// Gets the assembly names of the assemblies matching the filter.
            /// </summary>
            /// <param name="filenames">The filenames.</param>
            /// <param name="filter">The filter.</param>
            /// <returns>All assembly names of the assemblies matching the filter.</returns>
            public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames)
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

                    if (assembly.GetTypes().Any(t => t.IsClass && !t.IsAbstract && typeof (Plugin).IsAssignableFrom(t)))
                    {
                        result.Add(assembly.GetName(false));
                    }
                }

                return result;
            }
        }
    }
}
