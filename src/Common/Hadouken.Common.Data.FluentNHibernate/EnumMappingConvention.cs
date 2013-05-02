using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;

namespace Hadouken.Common.Data.FluentNHibernate
{
    internal class EnumMappingConvention : IUserTypeConvention
    {
        public void Accept(global::FluentNHibernate.Conventions.AcceptanceCriteria.IAcceptanceCriteria<global::FluentNHibernate.Conventions.Inspections.IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Property.PropertyType.IsEnum);
        }

        public void Apply(global::FluentNHibernate.Conventions.Instances.IPropertyInstance instance)
        {
            instance.CustomType(instance.Property.PropertyType);
        }
    }
}
