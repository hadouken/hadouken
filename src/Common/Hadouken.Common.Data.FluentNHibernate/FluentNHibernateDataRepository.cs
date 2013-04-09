using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NLog;

namespace Hadouken.Common.Data.FluentNHibernate
{
    [Component]
    public class FluentNHibernateDataRepository : IDataRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ISessionFactory _sessionFactory;
        private ISession _session;

        private readonly IEnvironment _environment;

        public FluentNHibernateDataRepository(IEnvironment environment)
        {
            _environment = environment;

            BuildSessionFactory();
        }

        private void BuildSessionFactory()
        {
            Logger.Debug("Creating the ISessionFactory with connection string {0}.", _environment.ConnectionString);

            _sessionFactory = Fluently.Configure()
                                      .Database(SQLiteConfiguration.Standard.ConnectionString(_environment.ConnectionString))
                                      .BuildSessionFactory();

            Logger.Debug("Opening an ISession to use.");

            _session = _sessionFactory.OpenSession();
        }

        public void Save<TModel>(TModel model) where TModel : Model, new()
        {
            _session.Save(model);
            _session.Flush();
        }

        public void SaveOrUpdate<TModel>(TModel model) where TModel : Model, new()
        {
            _session.SaveOrUpdate(model);
            _session.Flush();
        }

        public void Update<TModel>(TModel model) where TModel : Model, new()
        {
            _session.Update(model);
            _session.Flush();
        }

        public void Delete<TModel>(TModel model) where TModel : Model, new()
        {
            _session.Delete(model);
            _session.Flush();
        }

        public TModel Single<TModel>(int id) where TModel : Model, new()
        {
            return _session.Get<TModel>(id);
        }

        public TModel Single<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> query) where TModel : Model, new()
        {
            return _session.Query<TModel>().Where(query).SingleOrDefault();
        }

        public IEnumerable<TModel> List<TModel>() where TModel : Model, new()
        {
            return _session.Query<TModel>().ToList();
        }

        public IEnumerable<TModel> List<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> query) where TModel : Model, new()
        {
            return _session.Query<TModel>().Where(query).ToList();
        }
    }
}
