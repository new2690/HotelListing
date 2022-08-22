using Microsoft.AspNetCore.Identity;

namespace HotelListing.Models
{
    public class ApiUserRole:IdentityUserRole<int>
    {
        public ApiUser User { get; set; }
        public ApiRole Role { get; set; }
    }
}
