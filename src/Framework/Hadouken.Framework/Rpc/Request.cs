using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public class Request : IRequest
    {
        private readonly string _parameterAsJson;

        public Request(string parameterAsJson)
        {
            _parameterAsJson = parameterAsJson;
        }

        public object Id { get; set; }

        public string Method { get; set; }

        public string Protocol { get; set; }

        public T GetParameterObject<T>()
        {
            return JsonConvert.DeserializeObject<T>(_parameterAsJson);
        }
    }
}
