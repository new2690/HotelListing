using HotelListing.Models;
using System.Linq.Expressions;
using X.PagedList;

namespace HotelListing.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task RemoveAsync(int id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update(T entity);

        Task<IPagedList<T>> GetAllWithPagingAsync(Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            List<string> includes = null, RequestPagingParams request=null);

        Task<T> GetAsync(Expression<Func<T, bool>> expression = null, List<string> includes = null);

    }
}
