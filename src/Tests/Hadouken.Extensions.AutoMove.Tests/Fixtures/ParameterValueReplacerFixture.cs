namespace Hadouken.Extensions.AutoMove.Tests.Fixtures {
    internal sealed class ParameterValueReplacerFixture {
        public ParameterValueReplacerFixture() {
            this.SourceValueProvider = new SourceValueProvider();
        }

        public ISourceValueProvider SourceValueProvider { get; set; }

        public ParameterValueReplacer CreateReplacer() {
            return new ParameterValueReplacer(this.SourceValueProvider);
        }
    }
}