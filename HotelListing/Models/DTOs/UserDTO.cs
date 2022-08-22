using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models.DTOs
{
    public class LoginDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 25, MinimumLength = 8, ErrorMessage = "Maximum length is {1} and minimum is {2}")]
        public string Password { get; set; }
    }

    public class UserDTO:LoginDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
