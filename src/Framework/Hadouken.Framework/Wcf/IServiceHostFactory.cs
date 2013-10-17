using System;

namespace Hadouken.Framework.Wcf
{
    public interface IServiceHostFactory<T>
    {
        IServiceHost Create(Uri listenEndpoint);

        IServiceHost Create(Uri listenEndpoint, Type serviceType);
    }
}
