using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Metadata
{
    public abstract class SemanticVersionRange
    {
        public abstract bool IsIncluded(SemanticVersion version);

        public static SemanticVersionRange Construct(string range)
        {
            if(String.IsNullOrEmpty(range))
                return new LatestVersionRange();

            string[] parts = range.Split(',');
            char firstCharacter = range[0];

            return null;
        }
    }

    public class LatestVersionRange : SemanticVersionRange
    {
        public override bool IsIncluded(SemanticVersion version)
        {
            return true;
        }
    }

    public class GreaterThanOrEqualsRange : SemanticVersionRange
    {
        private readonly SemanticVersion _version;

        public GreaterThanOrEqualsRange(SemanticVersion version)
        {
            _version = version;
        }

        public override bool IsIncluded(SemanticVersion version)
        {
            return _version >= version;
        }
    }

    public class LessThanOrEqualsRange : SemanticVersionRange
    {


        public override bool IsIncluded(SemanticVersion version)
        {
            throw new NotImplementedException();
        }
    }

}
