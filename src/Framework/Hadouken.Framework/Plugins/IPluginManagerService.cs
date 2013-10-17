using System.ServiceModel;
using System.Threading.Tasks;

namespace Hadouken.Framework.Plugins
{
    [ServiceContract]
    public interface IPluginManagerService
    {
        [OperationContract]
        Task<string> RpcAsync(string request);

        [OperationContract]
        Task<byte[]> GetFileAsync(string path);
    }
}
