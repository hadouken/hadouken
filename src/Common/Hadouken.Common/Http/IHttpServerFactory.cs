using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Reflection;

namespace Hadouken.Common.Http
{
    public interface IHttpServerFactory
    {
        IHttpFileSystemServer Create(string baseAddress, NetworkCredential credential, string path);
        IHttpWebApiServer Create(string baseAddress, NetworkCredential credential, Assembly[] controllerAssemblies);
    }
}
