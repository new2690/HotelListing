using Microsoft.AspNetCore.Identity;

namespace HotelListing.Models
{
    public class ApiRole:IdentityRole<int>
    {
        public IEnumerable<ApiUserRole> UserRoles { get; set; }
    }
}
