using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models.DTOs
{
    public class CreateHotelDTO
    {
        [Required]
        [StringLength(maximumLength: 200, ErrorMessage = "The maximum {0} length is {1}")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 300, ErrorMessage = "The maximum {0} length is {1}")]
        public string Address { get; set; }

        [Required]
        [Range(1.0,5.0,ErrorMessage ="The range {0} is between {1} and {2}")]
        public string Rating { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }
        public CountryDTO Country { get; set; }
    }

    public class UpdateHotelDto : CreateHotelDTO
    {

    }
}
