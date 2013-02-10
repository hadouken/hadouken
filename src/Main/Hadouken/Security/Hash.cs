using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hadouken.Security
{
    public sealed class Hash
    {   
        /// <summary>
        /// Generate a hash using the specified HashAlgorithm.
        /// </summary>
        public static string Generate<TProvider>(string data) where TProvider : HashAlgorithm
        {
            if (String.IsNullOrEmpty(data))
                return null;

            var algo = Activator.CreateInstance<TProvider>();
            var hash = algo.ComputeHash(Encoding.UTF8.GetBytes(data));

            return BitConverter.ToString(hash).Replace("-", "");
        }

        /// <summary>
        /// Generate a hash using the default SHA512Managed algorithm.
        /// </summary>
        public static string Generate(string data)
        {
            return Generate<SHA512Managed>(data);
        }
    }
}
