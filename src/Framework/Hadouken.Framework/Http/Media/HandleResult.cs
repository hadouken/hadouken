using System.Net;

namespace Hadouken.Framework.Http.Media
{
    public abstract class HandleResult
    {
        public abstract void Execute(HttpListenerContext context);
    }
}
