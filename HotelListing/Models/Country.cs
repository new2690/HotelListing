using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        // Relationships
        public virtual IList<Hotel> Hotels { get; set; }

        
    }
}
