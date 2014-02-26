using Hadouken.Fx.Bootstrapping.TinyIoC;
using Hadouken.Fx.JsonRpc;
using Hadouken.Plugins.Sample.JsonRpc;

namespace Hadouken.Plugins.Sample.Bootstrapping
{
    public class CustomBootstrapper : TinyIoCBootstrapper
    {
        protected override void RegisterComponents(TinyIoCContainer container)
        {
            base.RegisterComponents(container);

            container.RegisterMultiple<IJsonRpcService>(new[] {typeof (MathService)});
        }
    }
}
