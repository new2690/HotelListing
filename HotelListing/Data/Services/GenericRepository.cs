using HotelListing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task Add(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>> expression = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            var query = _db.AsQueryable();

            if (expression != null)
                query = query.Where(expression);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>> expression = null, List<string> includes = null)
        {
            var query = _db.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(expression);
        }

        public void Remove(T entity)
        {
            _db.Remove(entity);
        }

        public async Task Remove(int id)
        {
            var entity =await _db.FindAsync(id);
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
