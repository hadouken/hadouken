using System.ServiceModel;

namespace Hadouken.Fx.ServiceModel
{
    [ServiceContract]
    public interface IPluginService
    {
        [OperationContract]
        string HandleJsonRpc(string request);
    }
}