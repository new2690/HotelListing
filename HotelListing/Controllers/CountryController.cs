using AutoMapper;
using HotelListing.Data.Interfaces;
using HotelListing.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork,ILogger<CountryController> logger,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReqiredAdmin")]
        public async Task<IActionResult> GetAllCountry()
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetAllAsync(includes:new List<string>() { "Hotels" });
                var result = _mapper.Map<IEnumerable<CountryDTO>>(countries);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"An error accure during call {nameof(GetAllCountry)} method");
                return StatusCode(500, "Internal Server Error. Plase Try Later");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReqiredUser")]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                var country =await _unitOfWork.Countries.GetAsync(c => c.Id == id,includes:new List<string> { "Hotels"});
                var result=_mapper.Map<CountryDTO>(country);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(GetCountry)} method");
                return StatusCode(500, "Internal Server Error. Please Try Later");
            }
        }
    }
}
