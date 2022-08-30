using AspNetCoreRateLimit;
using FluentValidation;
using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.Data.Interfaces;
using HotelListing.Data.Services;
using HotelListing.Models;
using HotelListing.Models.DTOs;
using HotelListing.Validations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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

            services.AddScoped<IValidator<CreateHotelDTO>, HotelValidator>();

            services.AddScoped<IValidator<CreateCountryDTO>, CountryValidator>();

            services.AddAutoMapper(typeof(MapperInitializer));

            services.AddControllersWithViews(options =>
            {
                options.CacheProfiles.Add("60Cache", new CacheProfile
                {
                    Duration=60
                });
            })
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
                opt.AddPolicy("ReqiredUser", pol => pol.RequireRole("Administrator", "User"));
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


        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
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


        public static void ConfigureExceptionHandler(this IApplicationBuilder builder)
        {
            builder.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        Log.Error($"An error accure during call {contextFeature.Error} method. Custom error register it.");

                        await context.Response.WriteAsync(new CustomError
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal server error. Custom error register it. Please try again later"

                        }.ToString());
                    }
                });
            });
        }


        public static void ConfigureApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;

                opt.AssumeDefaultVersionWhenUnspecified = true;

                opt.DefaultApiVersion = new ApiVersion(2,0);
            });
        }


        public static void ConfigureRateLimit(this IServiceCollection services, IConfiguration configuration)
        {
            // needed to load configuration from appsettings.json
            services.AddOptions();

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            services.AddInMemoryRateLimiting();
            //services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
            //services.AddDistributedRateLimiting<RedisProcessingStrategy>();
            //services.AddRedisRateLimiting();

            // Add framework services.
            //services.AddMvc();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // inject counter and rules distributed cache stores
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
        }
    }
}
