namespace Hadouken.SemVer
{
    public class EqualsRule : Rule
    {
        private readonly SemanticVersion _version;

        public EqualsRule(SemanticVersion version)
        {
            _version = version;
        }

        public override bool IsIncluded(SemanticVersion version)
        {
            return version == _version;
        }
    }
}