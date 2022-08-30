using HotelListing.Data.Interfaces;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using X.PagedList;

namespace HotelListing.Data.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public async Task<IPagedList<T>> GetAllWithPagingAsync(Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null,
            RequestPagingParams requestParams=null)
        {
            var query = _db.AsQueryable();

            if (expression != null)
                query = query.Where(expression);
            
            if (includes != null)
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

            if (orderBy != null)
                query = orderBy(query);

            return await query.AsNoTracking()
                .ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);

        }


        public async Task<T> GetAsync(Expression<Func<T, bool>> expression = null, List<string> includes = null)
        {
            var query = _db.AsQueryable();

            if (includes != null)
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public void Remove(T entity)
        {
            _db.Remove(entity);
        }

        public async Task RemoveAsync(int id)
        {
            var entity = await _db.FindAsync(id);
            if (entity != null)
                Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
