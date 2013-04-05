using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Hadouken.Common.Data
{
    public interface IRepository<TModel> where TModel : Model
    {
        void Save(TModel model);
        void SaveOrUpdate(TModel model);
        void Update(TModel model);
        void Delete(TModel model);

        TModel Single(int id);
        TModel Single(Expression<Func<TModel, bool>> query);

        IEnumerable<TModel> List();
        IEnumerable<TModel> List(Expression<Func<TModel, bool>> query);
    }
}
