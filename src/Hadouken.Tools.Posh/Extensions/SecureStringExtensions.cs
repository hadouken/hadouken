using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Hadouken.Tools.Posh.Extensions
{
    public static class SecureStringExtensions
    {
        public static string ToUnsecureString(this SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException("secureString");

            var unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
