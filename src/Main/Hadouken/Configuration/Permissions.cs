using System;

namespace Hadouken.Configuration
{
    [Flags]
    public enum Permissions
    {
        None = 0,
        Read = 1,
        Write = 2
    }
}