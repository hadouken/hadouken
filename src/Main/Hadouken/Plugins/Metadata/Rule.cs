namespace Hadouken.Plugins.Metadata
{
    public abstract class Rule
    {
        public abstract bool IsIncluded(SemanticVersion version);
    }
}