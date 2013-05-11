using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Hadouken.Data
{
    public interface IDataRepository
    {
        void Save<TModel>(TModel instance) where TModel : Model, new();
        void SaveOrUpdate<TModel>(TModel instance) where TModel : Model, new();
        void Update<TModel>(TModel instance) where TModel : Model, new();
        void Delete<TModel>(TModel instance) where TModel : Model, new();

        TModel Single<TModel>(int id) where TModel : Model, new();
        TModel Single<TModel>(Expression<Func<TModel, bool>> where) where TModel : Model, new();

        IList<TModel> List<TModel>() where TModel : Model, new();
        IList<TModel> List<TModel>(Expression<Func<TModel, bool>> where) where TModel : Model, new();
    }
}
