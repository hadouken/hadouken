using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace Hadouken.Common.Data.FluentNHibernate
{
    [Component]
    public class FluentNHibernateDataRepository : IDataRepository
    {
        private ISessionFactory _sessionFactory;
        private ISession _session;

        public FluentNHibernateDataRepository()
        {
            BuildSessionFactory();
        }

        private void BuildSessionFactory()
        {
            _sessionFactory = Fluently.Configure()
                                      .Database(SQLiteConfiguration.Standard.ConnectionString("Data Source=:memory:"))
                                      .BuildSessionFactory();

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
