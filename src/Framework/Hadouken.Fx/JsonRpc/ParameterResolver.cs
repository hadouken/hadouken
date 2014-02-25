using System;
using System.Collections.Generic;
using System.Linq;

namespace Hadouken.Fx.JsonRpc
{
    public class ParameterResolver : IParameterResolver
    {
        private readonly IJsonSerializer _jsonSerializer;

        public ParameterResolver(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public object[] Resolve(object requestParameters, IParameter[] targetParameters)
        {
            if (targetParameters == null || !targetParameters.Any())
            {
                return null;
            }

            if (requestParameters == null && targetParameters.Any())
            {
                throw new InvalidOperationException("No request parameters when expected.");
            }

            if (requestParameters != null && targetParameters.Count() == 1)
            {
                // One request parameter and one target parameter. Simple stuff.
                var parameterJson = _jsonSerializer.Serialize(requestParameters);
                var parameterObject = _jsonSerializer.Deserialize(parameterJson, targetParameters[0].ParameterType);

                if (parameterObject == null)
                {
                    throw new InvalidOperationException(
                        "Could not convert request parameter to the required target type");
                }

                if (parameterObject.GetType() != targetParameters[0].ParameterType)
                {
                    throw new InvalidOperationException("Could not convert request parameter to target type.");
                }

                return new[] {parameterObject};
            }

            if (IsArray(requestParameters) && targetParameters.Length > 1)
            {
                var array = requestParameters as object[];

                if (array == null)
                {
                    throw new InvalidOperationException("Could not convert request parameters to array");
                }

                if (array.Length != targetParameters.Length)
                {
                    throw new InvalidOperationException("Request parameter count not equal to expected parameter count");
                }

                var result = new List<object>();

                for (var i = 0; i < targetParameters.Length; i++)
                {
                    var arg = array[i];
                    var param = targetParameters[i];

                    var argJson = _jsonSerializer.Serialize(arg);
                    var argObject = _jsonSerializer.Deserialize(argJson, param.ParameterType);

                    result.Add(argObject);
                }

                return result.ToArray();
            }

            throw new NotImplementedException();
        }

        private static bool IsArray(object obj)
        {
            return obj.GetType().IsArray;
        }
    }
}