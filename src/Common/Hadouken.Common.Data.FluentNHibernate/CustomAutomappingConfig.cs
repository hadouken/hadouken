using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;

namespace Hadouken.Common.Data.FluentNHibernate
{
    internal class CustomAutomappingConfig : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return typeof (Model).IsAssignableFrom(type);
        }
    }
}
