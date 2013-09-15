using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Hadouken.Framework.Http
{
    public interface IHttpFileServer
    {
        void Open();

        void Close();
    }
}
