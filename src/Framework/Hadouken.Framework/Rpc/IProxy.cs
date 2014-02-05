using System;

namespace Hadouken.Framework.Rpc
{
    public interface IProxy<out T> : IDisposable
    {
        T Channel { get; }
    }
}
