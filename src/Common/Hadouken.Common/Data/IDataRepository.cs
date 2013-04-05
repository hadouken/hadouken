using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Hadouken.Common.Data
{
    public interface IDataRepository
    {
        void Save<TModel>(TModel model) where TModel : Model, new();
        void SaveOrUpdate<TModel>(TModel model) where TModel : Model, new();
        void Update<TModel>(TModel model) where TModel : Model, new();
        void Delete<TModel>(TModel model) where TModel : Model, new();

        TModel Single<TModel>(int id) where TModel : Model, new();
        TModel Single<TModel>(Expression<Func<TModel, bool>> query) where TModel : Model, new();

        IEnumerable<TModel> List<TModel>() where TModel : Model, new();
        IEnumerable<TModel> List<TModel>(Expression<Func<TModel, bool>> query) where TModel : Model, new();
    }
}
