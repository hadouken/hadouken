using Hadouken.Extensions.AutoMove.Data;

namespace Hadouken.Extensions.AutoMove.Tests.Fixtures
{
    internal sealed class RuleFinderFixture
    {
        public RuleFinderFixture()
        {
            AutoMoveRepository = new AutoMoveRepositoryFixture().CreateRepository();
            SourceValueProvider = new SourceValueProvider();
        }

        public IAutoMoveRepository AutoMoveRepository { get; set; }

        public ISourceValueProvider SourceValueProvider { get; set; }

        public RuleFinder CreateRuleFinder()
        {
            return new RuleFinder(AutoMoveRepository, SourceValueProvider);
        }
    }
}
