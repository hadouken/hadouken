using System;
using System.Runtime.InteropServices;

namespace Hadouken.PluginHostProcess
{
    [Flags]
    internal enum ErrorModes : uint
    {
        SYSTEM_DEFAULT = 0x0,
        SEM_FAILCRITICALERRORS = 0x0001,
        SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
        SEM_NOGPFAULTERRORBOX = 0x0002,
        SEM_NOOPENFILEERRORBOX = 0x8000
    }

    internal static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern ErrorModes SetErrorMode(ErrorModes mode);
    }
}
