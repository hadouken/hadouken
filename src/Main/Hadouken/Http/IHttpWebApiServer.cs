using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Http
{
    public interface IHttpWebApiServer
    {
        Task OpenAsync();

        Task CloseAsync();
    }
}
