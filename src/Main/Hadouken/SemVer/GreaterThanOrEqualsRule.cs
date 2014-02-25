namespace Hadouken.SemVer
{
    public class GreaterThanOrEqualsRule : Rule
    {
        private readonly SemanticVersion _version;

        public GreaterThanOrEqualsRule(SemanticVersion version)
        {
            _version = version;
        }

        public override bool IsIncluded(SemanticVersion version)
        {
            return version >= _version;
        }
    }
}