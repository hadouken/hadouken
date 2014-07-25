using System;
using System.Collections.Generic;

namespace Hadouken.Core.Http.Security.Storage
{
    /// <summary>
    /// Stores and retrieves encryption keys
    /// </summary>
    public interface ITokenKeyStore
    {
        /// <summary>
        /// Retrieves encryption keys
        /// </summary>
        /// <returns>Keys</returns>
        IDictionary<DateTime, byte[]> Retrieve();

        /// <summary>
        /// Stores encryption keys
        /// </summary>
        /// <param name="keys">Keys</param>
        void Store(IDictionary<DateTime, byte[]> keys);

        /// <summary>
        /// Purges encryption keys
        /// </summary>
        void Purge();
    }
}
