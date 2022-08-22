using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.Data.Interfaces;
using HotelListing.Data.Services;
using HotelListing.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HotelListing.Extensions
{
    public static class ServiceExtensions
    {
        public static void Configurations(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(MapperInitializer));
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            // Identity config

            services.AddAuthentication();

            //var builder = 
            services.AddIdentityCore<ApiUser>(u => u.User.RequireUniqueEmail = true)
            .AddRoles<ApiRole>()
            .AddRoleManager<RoleManager<ApiRole>>()
            .AddSignInManager<SignInManager<ApiUser>>()
            .AddRoleValidator<RoleValidator<ApiRole>>()
            .AddEntityFrameworkStores<DatabaseContext>();

            // IdentityRole refers to user is admin or guest or ...

            //builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);

            // we need tell it which data should store. 
            //builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("ReqiredAdmin", pol => pol.RequireRole("Administrator"));
                opt.AddPolicy("ReqiredUser", pol => pol.RequireRole("Administrator","User"));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Policy config to allow request to this api and get services.

            services.AddCors(options => options.AddPolicy("hotelPolicy", cp =>
            cp.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));
        }

        public static void ConfigureJwt(this IServiceCollection services,IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            var key = Environment.GetEnvironmentVariable("KEYHotel", EnvironmentVariableTarget.Machine) ?? "";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters =
                new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    
                    // ValidateIssuer is for API server
                    ValidateIssuer = false,

                    // ValidateIssuer is for Angular app
                    ValidateAudience = false,

                    ValidIssuer = jwtSettings.GetSection("Issuer").Value
                };
            });

        }
    }
}
