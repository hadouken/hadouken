using System;

namespace Hadouken.Plugins.Metadata
{
    [Serializable]
    public class UnexpectedPropertyException : Exception
    {
        private readonly string _propertyName;

        public UnexpectedPropertyException(string propertyName)
        {
            _propertyName = propertyName;
        }   

        public string PropertyName
        {
            get { return _propertyName; }
        }
    }
}
