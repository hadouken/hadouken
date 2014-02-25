namespace Hadouken.SemVer
{
    public abstract class Rule
    {
        public abstract bool IsIncluded(SemanticVersion version);
    }
}