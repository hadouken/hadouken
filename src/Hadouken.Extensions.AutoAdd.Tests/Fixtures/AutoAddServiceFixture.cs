using Hadouken.Extensions.AutoAdd.Data;
using Hadouken.Extensions.AutoAdd.Services;
using NSubstitute;

namespace Hadouken.Extensions.AutoAdd.Tests.Fixtures
{
    internal sealed class AutoAddServiceFixture
    {
        public AutoAddServiceFixture()
        {
            AutoAddRepository = Substitute.For<IAutoAddRepository>();
        }

        public IAutoAddRepository AutoAddRepository { get; set; }

        public AutoAddService CreateAutoAddService()
        {
            return new AutoAddService(AutoAddRepository);
        }
    }
}
