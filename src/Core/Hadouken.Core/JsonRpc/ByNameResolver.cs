using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Text;

namespace Hadouken.Core.JsonRpc {
    public class ByNameResolver : IParameterResolver {
        private readonly IJsonSerializer _serializer;

        public ByNameResolver(IJsonSerializer serializer) {
            if (serializer == null) {
                throw new ArgumentNullException("serializer");
            }
            this._serializer = serializer;
        }

        public bool CanResolve(object requestParameters) {
            // The request parameters must be a string-object dictionary
            return (requestParameters as IDictionary<string, object>) != null;
        }

        public object[] Resolve(object requestParameters, IParameter[] targetParameters) {
            var parameters = (IDictionary<string, object>) requestParameters;

            // If length does not match, throw.
            if (parameters.Keys.Count != targetParameters.Length) {
                throw new ParameterLengthMismatchException();
            }

            var result = new List<object>();

            // Loop through all keys and check if we have the parameter.
            foreach (var pair in parameters) {
                // Find the target parameter with a name equal to the one in the request
                var target = targetParameters.SingleOrDefault(tp => string.Equals(pair.Key, tp.Name));

                // We did not find a parameter with this name, throw.
                if (target == null) {
                    throw new ParameterNameNotFoundException();
                }

                if (target.ParameterType == typeof (object)) {
                    result.Add(pair.Value);
                }
                else {
                    var serialized = this._serializer.SerializeObject(pair.Value);
                    var converted = this._serializer.DeserializeObject(serialized, target.ParameterType);

                    result.Add(converted);
                }
            }

            return result.ToArray();
        }
    }
}