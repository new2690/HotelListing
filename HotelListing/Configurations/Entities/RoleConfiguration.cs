using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Configurations.Entities
{
    public class RoleConfiguration : IEntityTypeConfiguration<ApiRole>
    {
        public void Configure(EntityTypeBuilder<ApiRole> builder)
        {
            builder.HasData(new ApiRole
            {
                Id=1,
                Name = "User",
                NormalizedName = "USER"
            }, new ApiRole
            {
                Id = 2, 
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            });
        }
    }
}
