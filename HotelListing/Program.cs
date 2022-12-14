using AspNetCoreRateLimit;
using HotelListing.Data;
using HotelListing.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog config

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext().CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
Log.Logger = logger;


// Add services to the container.

builder.Services.ConfigureRateLimit(builder.Configuration);

//builder.Services.AddHttpContextAccessor();

builder.Services.Configurations();

builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.ConfigureApiVersioning();



builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("hotelConnection"));
});

builder.Services.AddResponseCaching();

try
{
    Log.Information("My Application is starting");
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    app.UseCors("hotelPolicy");
    
    app.UseResponseCaching();

    app.UseIpRateLimiting();

    app.ConfigureExceptionHandler();

    app.UseHttpsRedirection();
    
    app.UseAuthentication();
    
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    Log.Information("finally log");
    Log.CloseAndFlush();
}