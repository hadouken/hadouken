namespace Hadouken.SemVer
{
    public class GreaterThanRule : Rule
    {
        private readonly SemanticVersion _version;

        public GreaterThanRule(SemanticVersion version)
        {
            _version = version;
        }

        public override bool IsIncluded(SemanticVersion version)
        {
            return version > _version;
        }
    }
}