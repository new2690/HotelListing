using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(new Hotel
            {
                Id = 1,
                Name = "Irani Hotel",
                Address = "Tehran khyabane aval",
                Rating = 3.8,
                CountryId = 1
            }, new Hotel
            {
                Id = 2,
                Name = "American Hotel",
                Address = "New york city street 2",
                Rating = 4.8,
                CountryId = 2
            }, new Hotel
            {
                Id = 3,
                Name = "Franchi Hotel",
                Address = "Paris, street 10",
                Rating = 4.8,
                CountryId = 3
            }, new Hotel
            {
                Id = 4,
                Name = "Greman Hotel",
                Address = "Berlin square azadi",
                Rating = 4.1,
                CountryId = 4
            }, new Hotel
            {
                Id = 5,
                Name = "Austria People Hotel",
                Address = "Center city",
                Rating = 4.5,
                CountryId = 5
            }, new Hotel
            {
                Id = 6,
                Name = "America Rest hotel",
                Address = "WD seconde street",
                Rating = 4.5,
                CountryId = 2
            }, new Hotel
            {
                Id = 7,
                Name = "Sanandaj shadi hotel",
                Address = "Baharan",
                Rating = 4.1,
                CountryId = 1
            });
        }
    }
}
