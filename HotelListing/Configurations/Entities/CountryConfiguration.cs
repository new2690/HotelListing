using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Configurations.Entities
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(new Country
            {
                Id = 1,
                Name = "Iran",
                ShortName = "IR"
            }, new Country
            {
                Id = 2,
                Name = "United State",
                ShortName = "USA"
            }, new Country
            {
                Id = 3,
                Name = "France",
                ShortName = "FR"
            }, new Country
            {
                Id = 4,
                Name = "Germany",
                ShortName = "GE"
            }, new Country
            {
                Id = 5,
                Name = "Austria",
                ShortName = "AU"
            });
        }
    }
}
