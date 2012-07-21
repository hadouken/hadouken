using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Hadouken.Data
{
    public interface IDataRepository : IComponent
    {
        void Save<TModel>(TModel instance) where TModel : IModel, new();
        void SaveOrUpdate<TModel>(TModel instance) where TModel : IModel, new();
        void Update<TModel>(TModel instance) where TModel : IModel, new();
        void Delete<TModel>(TModel instance) where TModel : IModel, new();

        TModel Single<TModel>(int id) where TModel : IModel, new();
        TModel Single<TModel>(Expression<Func<TModel, bool>> where) where TModel : IModel, new();

        IList<TModel> List<TModel>() where TModel : IModel, new();
        IList<TModel> List<TModel>(Expression<Func<TModel, bool>> where) where TModel : IModel, new();
    }
}
