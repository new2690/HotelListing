using System.Linq.Expressions;

namespace HotelListing.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task Add(T entity);
        Task AddRange(IEnumerable<T> entities);
        Task Remove(int id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update(T entity);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null);
        Task<T> GetAsync(Expression<Func<T, bool>> expression = null, List<string> includes = null);
    }
}
