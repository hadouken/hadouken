using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Framework.Rpc
{
    public interface IParameterResolver
    {
        object[] Resolve(JToken input, IMethodInvoker method);
    }

    public class ParameterResolver : IParameterResolver
    {
        public object[] Resolve(JToken input, IMethodInvoker method)
        {
            var paramCount = method.Parameters.Length;

            if (paramCount == 0 && input == null)
            {
                return null;
            }
            
            if (paramCount == 1)
            {
                var paramType = method.Parameters[0].ParameterType;

                try
                {
                    return new[] {input.ToObject(paramType)};
                }
                catch (Exception e)
                {
                    throw new InvalidParametersException("Could not parse parameters.", e);
                }
            }

            // If we get this far, we either have an array with parameters, or an object with named parameters.
            // find out which.

            switch (input.Type)
            {
                case JTokenType.Array:
                    return ResolveArray(input as JArray, method);

                case JTokenType.Object:
                    return ResolveObject(input as JObject, method);
            }

            throw new InvalidParametersException();
        }

        private object[] ResolveObject(JObject obj, IMethodInvoker method)
        {
            var result = new List<object>();
            var d = obj as IDictionary<string, JToken>;

            if (d == null)
                throw new Exception();

            if (d.Keys.Count != method.Parameters.Length)
                throw new InvalidParametersException();

            foreach (var param in method.Parameters)
            {
                var argName = param.Name;

                if (d.ContainsKey(argName))
                {
                    result.Add(d[argName].ToObject(param.ParameterType));
                }
                else
                {
                    throw new InvalidParametersException();
                }
            }

            return result.ToArray();
        }

        private object[] ResolveArray(JArray array, IMethodInvoker method)
        {
            if (array.Count != method.Parameters.Length)
                throw new InvalidParametersException();

            return method.Parameters.Select((param, i) => array[i].ToObject(param.ParameterType)).ToArray();
        }
    }
}
