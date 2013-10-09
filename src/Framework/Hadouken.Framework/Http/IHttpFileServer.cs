using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Hadouken.Framework.Http
{
    public interface IHttpFileServer
    {
        void SetCredentials(string username, string password);

        void Open();

        void Close();
    }
}
