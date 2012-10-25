using System;
using System.Reflection;

namespace Hadouken.Http.HttpServer
{
    public class ActionCacheItem
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public Type Controller { get; set; }
        public MethodInfo Action { get; set; }
    }
}
