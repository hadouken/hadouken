using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins.Web.Http
{
    public interface IHttpFileServer
    {
        void Start();

        void Stop();
    }
}
