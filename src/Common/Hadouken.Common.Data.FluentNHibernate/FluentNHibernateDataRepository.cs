using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace Hadouken.Common.Data.FluentNHibernate
{
    public class FluentNHibernateDataRepository : IDataRepository
    {
        private ISessionFactory _sessionFactory;

        public FluentNHibernateDataRepository()
        {
            BuildSessionFactory();
        }

        private void BuildSessionFactory()
        {
            _sessionFactory = Fluently.Configure()
                                      .Database(SQLiteConfiguration.Standard.ConnectionString(""))
                                      .BuildSessionFactory();
        }

        public void Save<TModel>(TModel model) where TModel : Model, new()
        {
            throw new NotImplementedException();
        }

        public void SaveOrUpdate<TModel>(TModel model) where TModel : Model, new()
        {
            throw new NotImplementedException();
        }

        public void Update<TModel>(TModel model) where TModel : Model, new()
        {
            throw new NotImplementedException();
        }

        public void Delete<TModel>(TModel model) where TModel : Model, new()
        {
            throw new NotImplementedException();
        }

        public TModel Single<TModel>(int id) where TModel : Model, new()
        {
            throw new NotImplementedException();
        }

        public TModel Single<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> query) where TModel : Model, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TModel> List<TModel>() where TModel : Model, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TModel> List<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> query) where TModel : Model, new()
        {
            throw new NotImplementedException();
        }
    }
}
