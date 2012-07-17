using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public interface IHttpServer : IComponent
    {
        void Start();
        void Stop();

        Uri ListenUri { get; }
    }
}
