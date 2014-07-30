using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;

namespace Hadouken.Core
{
    public class ExtensionFactory : IExtensionFactory
    {
        private readonly IKeyValueStore _keyValueStore;
        private readonly IList<IExtension> _extensions;

        public ExtensionFactory(IKeyValueStore keyValueStore, IEnumerable<IExtension> extensions)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
            _extensions = new List<IExtension>(extensions ?? Enumerable.Empty<IExtension>());
        }

        public IEnumerable<TExtension> GetAll<TExtension>() where TExtension : IExtension
        {
            return new List<TExtension>(_extensions.OfType<TExtension>());
        }

        public TExtension Get<TExtension>(string extensionId) where TExtension : IExtension
        {
            return _extensions.OfType<TExtension>().SingleOrDefault(e => e.GetId() == extensionId);
        }

        public bool IsEnabled(string extensionId)
        {
            var enabledExtensions = GetEnabledExtensions();
            return enabledExtensions.Contains(extensionId);
        }

        public void Enable(string extensionId)
        {
            var enabledExtensions = GetEnabledExtensions();
            if (enabledExtensions.Contains(extensionId)) return;

            enabledExtensions.Add(extensionId);
            _keyValueStore.Set("extensions.enabled", enabledExtensions);
        }

        public void Disable(string extensionId)
        {
            var enabledExtensions = GetEnabledExtensions();
            if (!enabledExtensions.Contains(extensionId)) return;

            enabledExtensions.Remove(extensionId);
            _keyValueStore.Set("extensions.enabled", enabledExtensions);
        }

        private IList<string> GetEnabledExtensions()
        {
            return _keyValueStore.Get<IList<string>>("extensions.enabled", new List<string>());            
        }
    }
}