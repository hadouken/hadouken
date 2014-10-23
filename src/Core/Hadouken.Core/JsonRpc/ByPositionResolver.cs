using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Text;

namespace Hadouken.Core.JsonRpc
{
    public class ByPositionResolver : IParameterResolver
    {
        private readonly IJsonSerializer _serializer;

        public ByPositionResolver(IJsonSerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            _serializer = serializer;
        }

        public bool CanResolve(object requestParameters)
        {
            // String is edge case
            if (requestParameters == null
                || requestParameters is string)
            {
                return false;
            }

            // The request parameters must be an array and not a dictionary
            var type = requestParameters.GetType();
            var isGenericDictionary = type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Dictionary<,>);

            return (requestParameters as IEnumerable != null)
                   && (requestParameters as IDictionary == null)
                   && !isGenericDictionary;
        }

        public object[] Resolve(object requestParameters, IParameter[] targetParameters)
        {
            // Convert to array
            var parameters = ((IEnumerable) requestParameters).Cast<object>().ToArray();

            // If length does not match, throw.
            if (parameters.Length != targetParameters.Length)
            {
                throw new ParameterLengthMismatchException();
            }

            var result = new List<object>();

            // Loop through and parse parameters
            for (var i = 0; i < parameters.Length; i++)
            {
                var request = parameters[i];
                var target = targetParameters[i];

                // If the target parameter is `object`, do not try to convert it.
                if (target.ParameterType == typeof (object))
                {
                    result.Add(request);
                }
                else
                {
                    var serialized = _serializer.SerializeObject(request);
                    var converted = _serializer.DeserializeObject(serialized, target.ParameterType);

                    result.Add(converted);
                }
            }

            return result.ToArray();
        }
    }
}