using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hadouken.Impl.Data;
using Moq;
using Hadouken.Messaging;
using Hadouken.Data.Models;
using System.IO;

namespace Hadouken.UnitTests.Data
{
    [TestFixture]
    public class DataRepositoryTests
    {
        private static System.Data.SQLite.SQLiteConnection __conn__;

        [TestFixtureSetUp]
        public void Setup()
        {
            if (File.Exists("test.db"))
                File.Delete("test.db");

            // apply migrations

            var runner = new DefaultMigratorRunner();
            runner.Run(AppDomain.CurrentDomain.Load("Hadouken.Impl"));
        }

        [Test]
        public void Can_CRUD_records()
        {
            var mbus = new Mock<IMessageBus>();

            // can save
            var repo = new FluentNhibernateDataRepository(mbus.Object);
            repo.Save(new Setting() { Key = "test", Value = "test" });

            Assert.IsTrue(repo.List<Setting>().Count == 1);

            // can update
            var s = repo.Single<Setting>(q => q.Key == "test");
            s.Key = "test2";

            repo.Update(s);

            Assert.IsTrue(repo.Single<Setting>(s.Id).Key == "test2");

            // can delete

            repo.Delete(s);

            Assert.IsTrue(repo.List<Setting>().Count == 0);
        }
    }
}
