using HotelListing.Models;

namespace HotelListing.Data.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApiUser token);
    }
}
