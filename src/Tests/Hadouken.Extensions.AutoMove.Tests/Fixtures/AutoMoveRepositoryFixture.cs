using Hadouken.Common.Data;
using Hadouken.Common.Logging;
using Hadouken.Common.Reflection;
using Hadouken.Extensions.AutoMove.Data;
using NSubstitute;

namespace Hadouken.Extensions.AutoMove.Tests.Fixtures {
    internal sealed class AutoMoveRepositoryFixture {
        public AutoMoveRepositoryFixture() {
            this.DbConnection = new DbConnection("Data Source=:memory:");
            this.RunMigrations = true;
        }

        public bool RunMigrations { get; set; }
        public IDbConnection DbConnection { get; set; }

        public AutoMoveRepository CreateRepository() {
            if (this.RunMigrations) {
                var migrator = new SqlMigrator(Substitute.For<ILogger<SqlMigrator>>(), this.DbConnection,
                    new EmbeddedResourceFinder());
                migrator.Migrate();
            }

            return new AutoMoveRepository(this.DbConnection);
        }
    }
}