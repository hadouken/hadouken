using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using System.Reflection;

namespace Hadouken.Impl.Http
{
    public class ActionCacheItem
    {
        public Type Controller { get; set; }
        public MethodInfo Action { get; set; }
    }
}
