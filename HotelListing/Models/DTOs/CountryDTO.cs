using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models.DTOs
{
    public class CreateCountryDTO
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "The maximum {0} length is {1}")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 3, ErrorMessage = "The maximum {0} length is {1}")]
        public string ShortName { get; set; }
    }
    public class CountryDTO:CreateCountryDTO
    {
        public int Id { get; set; }

        public IEnumerable<HotelDTO> Hotels { get; set; }
    }

    public class UpdateCountryDto : CreateCountryDTO { }
}
