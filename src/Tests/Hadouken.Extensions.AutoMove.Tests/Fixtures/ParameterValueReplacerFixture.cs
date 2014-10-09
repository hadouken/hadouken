namespace Hadouken.Extensions.AutoMove.Tests.Fixtures
{
    internal sealed class ParameterValueReplacerFixture
    {
        public ParameterValueReplacerFixture()
        {
            SourceValueProvider = new SourceValueProvider();
        }

        public ISourceValueProvider SourceValueProvider { get; set; }

        public ParameterValueReplacer CreateReplacer()
        {
            return new ParameterValueReplacer(SourceValueProvider);
        }
    }
}
