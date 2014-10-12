using Hadouken.Common.Data;
using Hadouken.Common.Logging;
using Hadouken.Common.Reflection;
using Hadouken.Extensions.AutoMove.Data;
using NSubstitute;

namespace Hadouken.Extensions.AutoMove.Tests.Fixtures
{
    internal sealed class AutoMoveRepositoryFixture
    {
        public AutoMoveRepositoryFixture()
        {
            DbConnection = new DbConnection("Data Source=:memory:");
            RunMigrations = true;
        }

        public bool RunMigrations { get; set; }

        public IDbConnection DbConnection { get; set; }

        public AutoMoveRepository CreateRepository()
        {
            if (RunMigrations)
            {
                var migrator = new SqlMigrator(Substitute.For<ILogger<SqlMigrator>>(),
                    DbConnection,
                    new EmbeddedResourceFinder());
                migrator.Migrate();
            }

            return new AutoMoveRepository(DbConnection);
        }
    }
}
