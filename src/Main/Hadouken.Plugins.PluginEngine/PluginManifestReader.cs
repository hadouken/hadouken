using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hadouken.Plugins.PluginEngine
{
    public class PluginManifestReader : MarshalByRefObject
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        static PluginManifestReader()
        {
            Serializer.Converters.Add(new VersionConverter());
        }

        public PluginManifest ReadManifest()
        {
            var pluginAssembly = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                  from name in asm.GetManifestResourceNames()
                                  where name.EndsWith("manifest.json")
                                  select new
                                      {
                                          Assembly = asm,
                                          Name = name
                                      }).First();

            using (var stream = pluginAssembly.Assembly.GetManifestResourceStream(pluginAssembly.Name))
            {
                if (stream == null)
                    throw new Exception();

                using (var reader = new StreamReader(stream))
                {
                    return Serializer.Deserialize<PluginManifest>(new JsonTextReader(reader));
                }
            }
        }

        internal void Load(IEnumerable<byte[]> assemblies)
        {
            foreach (var asm in assemblies)
            {
                AppDomain.CurrentDomain.Load(asm);
            }
        }
    }
}
