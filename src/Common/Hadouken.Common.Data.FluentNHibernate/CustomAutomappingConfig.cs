using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Data.FluentNHibernate
{
    public class CustomAutomappingConfig
    {
        public override bool ShouldMap(Type type)
        {
            return typeof (Model).IsAssignableFrom(type);
        }
    }
}
