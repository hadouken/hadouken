using System.ServiceModel.Channels;

namespace Hadouken.Framework.Wcf
{
    public interface IBindingBuilder
    {
        Binding Build(string bindingUri);
    }
}
