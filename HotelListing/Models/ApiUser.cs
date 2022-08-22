using Microsoft.AspNetCore.Identity;

namespace HotelListing.Models
{
    public class ApiUser:IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IEnumerable<ApiUserRole> UserRoles { get; set; }
    }
}
