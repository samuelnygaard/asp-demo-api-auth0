using System.Linq.Expressions;

namespace Data.Interfaces
{
    internal interface IRepository<TModel, TEntity, T> 
        where TModel : class
        where TEntity : class, IEntity<T>
    {
        Task<IEnumerable<TModel>> GetAll();
        Task<TModel?> Get(T id);
        Task<TModel> Add(TModel data);
        Task<TModel> Update(TModel data);
        Task<IEnumerable<TModel>> Find(Expression<Func<TModel, bool>> expression);
        Task Delete(T id);
        Task Delete(TModel data);
    }
}
