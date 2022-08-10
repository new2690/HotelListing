using HotelListing.Models;

namespace HotelListing.Data.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        public IGenericRepository<Hotel> Hotels { get; }
        public IGenericRepository<Country> Countries { get; }
        Task Save();
    }
}
