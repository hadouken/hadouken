using Hadouken.Extensions.AutoMove.Data;
using Hadouken.Extensions.AutoMove.Services;
using NSubstitute;

namespace Hadouken.Extensions.AutoMove.Tests.Fixtures
{
    internal sealed class AutoMoveServiceFixture
    {
        public AutoMoveServiceFixture()
        {
            AutoMoveRepository = Substitute.For<IAutoMoveRepository>();
        }

        public IAutoMoveRepository AutoMoveRepository { get; set; }

        public AutoMoveService CreateService()
        {
            return new AutoMoveService(AutoMoveRepository);
        }
    }
}
