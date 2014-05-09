using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Hadouken.PluginHostProcess
{
    public class PluginHost
    {
        private const string FxPlugin = "Hadouken.Fx.Plugin";
        private const string FxPluginConfiguration = "Hadouken.Fx.PluginConfiguration";
        private const string FxBootstrapperAttribute = "Hadouken.Fx.Bootstrapping.BootstrapperAttribute";
        private const string FxTinyIoCBootstrapper = "Hadouken.Fx.Bootstrapping.TinyIoC.TinyIoCBootstrapper";

        private object _pluginHost;

        public void Load(string id)
        {
            var files = Directory.GetFiles(".", "*.dll");
            foreach (var file in files)
            {
                Assembly.LoadFrom(file);
            }

            Type type = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from t in asm.GetTypes()
                where t.IsClass && !t.IsAbstract
                where t.BaseType != null && t.BaseType.FullName == FxPlugin
                select t).FirstOrDefault();

            if (type == null)
                return;

            var fxAssembly = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from t in asm.GetTypes()
                where t.FullName == FxPlugin
                select asm).Single();

            // Find bootstrapper
            var bootstrapperAttributeType = fxAssembly.GetType(FxBootstrapperAttribute, true, true);
            var bootstrapperAttribute = type.GetCustomAttribute(bootstrapperAttributeType);
            object bootstrapper;

            if (bootstrapperAttribute == null)
            {
                // Hard coded default bootstrapper
                var defaultBootstrapper = fxAssembly.GetType(FxTinyIoCBootstrapper);
                bootstrapper = Activator.CreateInstance(defaultBootstrapper);
            }
            else
            {
                // Use the plugin specified one
                var customBootstrapperType = bootstrapperAttribute.GetPropertyValue<Type>("Type");
                bootstrapper = Activator.CreateInstance(customBootstrapperType);
            }

            var conf = ReadConfig(fxAssembly, id);
            bootstrapper.Invoke("Initialize", new object[] {conf});

            // Use it to get the IPluginHost
            _pluginHost = bootstrapper.Invoke("GetHost");
            _pluginHost.Invoke("Load");
        }

        public void Unload()
        {
            _pluginHost.Invoke("Unload");
        }

        private static object ReadConfig(Assembly fxAssembly, string fileName)
        {
            using (var mmf = MemoryMappedFile.OpenExisting(fileName))
            using (var stream = mmf.CreateViewStream())
            using (var reader = new BinaryReader(stream))
            {
                var json = reader.ReadString();
                return new JavaScriptSerializer().Deserialize(json, fxAssembly.GetType(FxPluginConfiguration));
            }
        } 
    }
}
