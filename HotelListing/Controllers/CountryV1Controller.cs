using HotelListing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Controllers
{
    [ApiVersion("1.0",Deprecated =true)]
    [Route("api/{v:apiversion}/country")]
    [ApiController]
    public class CountryV1Controller : ControllerBase
    {
        private readonly DatabaseContext _context;

        public CountryV1Controller(DatabaseContext context)
        {
            _context = context;
        }


        [Authorize(Policy = "ReqiredAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAllCountry()
        {
            var result = await _context.Countries.ToListAsync();

            result.Add(new Models.Country
            {
                Id=20,
                Hotels=null,
                Name="New Countryyyyyyyyyyyyyyyyyyyyyyy",
                ShortName="NewWwWwWwWwWw"

            });
            return Ok(result);
            //return Ok("Get All country from api version 1 (deprycated)");
        }

    }
}
