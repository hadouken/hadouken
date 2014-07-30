using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hadouken.Common.Extensibility;
using Hadouken.Common.JsonRpc;
using Hadouken.Core.Services.Models;

namespace Hadouken.Core.Services
{
    public sealed class ExtensionService : IJsonRpcService
    {
        private readonly IExtensionFactory _extensionFactory;

        public ExtensionService(IExtensionFactory extensionFactory)
        {
            if (extensionFactory == null) throw new ArgumentNullException("extensionFactory");
            _extensionFactory = extensionFactory;
        }

        [JsonRpcMethod("extensions.getAll")]
        public IEnumerable<ExtensionMetadata> GetAll()
        {
            return (from ext in _extensionFactory.GetAll<IExtension>()
                let attr = ext.GetType().GetCustomAttribute<ExtensionAttribute>()
                select new ExtensionMetadata
                {
                    Id = attr.ExtensionId,
                    Name = attr.Name,
                    Description = attr.Description,
                    Enabled = _extensionFactory.IsEnabled(attr.ExtensionId)
                });
        }

        [JsonRpcMethod("extensions.disable")]
        public void DisableExtension(string extensionId)
        {
            _extensionFactory.Disable(extensionId);
        }

        [JsonRpcMethod("extensions.enable")]
        public void EnableExtension(string extensionId)
        {
            _extensionFactory.Enable(extensionId);
        }
    }
}
