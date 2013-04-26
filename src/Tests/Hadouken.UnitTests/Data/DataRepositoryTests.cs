using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Data.FluentNHibernate;
using Hadouken.Common.Messaging;
using NUnit.Framework;
using Hadouken.Impl.Data;
using Moq;
using Hadouken.Data.Models;
using System.IO;
using Hadouken.Configuration;
using Hadouken.Common;

namespace Hadouken.UnitTests.Data
{
    [TestFixture]
    public class DataRepositoryTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            HdknConfig.ConfigManager = new MemoryConfigManager();

            if (File.Exists("test.db"))
                File.Delete("test.db");

            // apply migrations

            var runner = new DefaultMigratorRunner();
            runner.Up(AppDomain.CurrentDomain.Load("Hadouken.Impl"));
        }

        [Test]
        public void Can_CRUD_records()
        {
            var env = new Mock<IEnvironment>();
            env.SetupGet(e => e.ConnectionString).Returns("Data Source=test.db; Version=3;");

            var repo = new FluentNHibernateDataRepository(env.Object);

            // Saving
            repo.Save(new Setting() { Key = "test", Value = "test" });
            Assert.IsTrue(repo.List<Setting>(st => st.Key == "test").Count() == 1);

            // Updating
            var s = repo.Single<Setting>(q => q.Key == "test");
            s.Key = "test2";

            repo.Update(s);
            Assert.IsTrue(repo.Single<Setting>(s.Id).Key == "test2");

            // Deleting
            repo.Delete(s);
            Assert.IsTrue(!repo.List<Setting>(st => st.Key == "test").Any());
        }
    }
}
