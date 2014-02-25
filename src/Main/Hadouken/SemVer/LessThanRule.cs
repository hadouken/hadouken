namespace Hadouken.SemVer
{
    public class LessThanRule : Rule
    {
        private readonly SemanticVersion _version;

        public LessThanRule(SemanticVersion version)
        {
            _version = version;
        }

        public override bool IsIncluded(SemanticVersion version)
        {
            return version < _version;
        }
    }
}