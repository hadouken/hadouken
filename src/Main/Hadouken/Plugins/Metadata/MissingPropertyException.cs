using System;

namespace Hadouken.Plugins.Metadata
{
    [Serializable]
    public class MissingPropertyException : Exception
    {
        private readonly string _fieldName;

        public MissingPropertyException(string fieldName)
        {
            _fieldName = fieldName;
        }

        public string FieldName
        {
            get { return _fieldName; }
        }
    }
}
