using HotelListing.Configurations.Entities;
using HotelListing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Data
{
    public class DatabaseContext :IdentityDbContext<ApiUser,ApiRole,int,IdentityUserClaim<int>,
        ApiUserRole,IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApiUser>()
                .HasMany<ApiUserRole>()
                .WithOne(r => r.User)
                .HasForeignKey(k => k.UserId)
                .IsRequired();

            builder.Entity<ApiRole>()
                .HasMany<ApiUserRole>()
                .WithOne(r => r.Role)
                .HasForeignKey(k => k.RoleId)
                .IsRequired();

            builder.ApplyConfiguration(new CountryConfiguration());
            builder.ApplyConfiguration(new HotelConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());

        }


    }
}
