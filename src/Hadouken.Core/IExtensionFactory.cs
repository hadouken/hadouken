using System.Collections.Generic;
using Hadouken.Common.Extensibility;

namespace Hadouken.Core
{
    public interface IExtensionFactory
    {
        /// <summary>
        /// Gets an enumerable of all the extensions of the given type.
        /// </summary>
        /// <typeparam name="TExtension">The type of extension to get.</typeparam>
        /// <returns></returns>
        IEnumerable<TExtension> GetAll<TExtension>() where TExtension : IExtension;

        TExtension Get<TExtension>(string extensionId) where TExtension : IExtension;

        bool IsEnabled(string extensionId);

        void Enable(string extensionId);

        void Disable(string extensionId);
    }
}
