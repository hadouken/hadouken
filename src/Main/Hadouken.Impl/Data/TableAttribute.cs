using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Impl.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; private set; }
    }
}
