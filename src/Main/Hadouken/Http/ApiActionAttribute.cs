using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiActionAttribute : Attribute
    {
        public ApiActionAttribute(string actionName)
        {
            Name = actionName; 
        }

        public string Name { get; private set; }
    }
}
