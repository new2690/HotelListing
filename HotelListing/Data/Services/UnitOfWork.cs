using HotelListing.Data.Interfaces;
using HotelListing.Models;

namespace HotelListing.Data.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private IGenericRepository<Hotel> _hotel;
        private IGenericRepository<Country> _country;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        public IGenericRepository<Hotel> Hotels => _hotel ?? new GenericRepository<Hotel>(_context);

        public IGenericRepository<Country> Countries => _country ?? new GenericRepository<Country>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
