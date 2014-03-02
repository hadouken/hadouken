namespace Hadouken.SemVer
{
    public class LessThanOrEqualsRule : Rule
    {
        private readonly SemanticVersion _version;

        public LessThanOrEqualsRule(SemanticVersion version)
        {
            _version = version;
        }

        public override bool IsIncluded(SemanticVersion version)
        {
            return version <= _version;
        }
    }
}