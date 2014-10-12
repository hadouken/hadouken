namespace Hadouken.Core.JsonRpc
{
    public class NullResolver : IParameterResolver
    {
        public bool CanResolve(object requestParameters)
        {
            return requestParameters == null;
        }

        public object[] Resolve(object requestParameters, IParameter[] targetParameters)
        {
            if (targetParameters == null || targetParameters.Length == 0)
            {
                return null;
            }

            throw new ParameterLengthMismatchException();
        }
    }
}
