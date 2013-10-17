using System;
using System.ServiceModel.Channels;

namespace Hadouken.Framework.Wcf
{
    public interface IBindingFactory
    {
        Binding Create(Uri listenUri);
    }
}
