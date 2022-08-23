using AutoMapper;
using HotelListing.Data.Interfaces;
using HotelListing.Models;
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

        [HttpGet("{id}",Name = "GetCountry")]
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


        [HttpPost]
        [Authorize(Policy = "ReqiredUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
        {
            _logger.LogInformation("Try to create new country");

            if (!ModelState.IsValid)
            {
                _logger.LogError($"Try to create new country was fail. method used was {nameof(CreateCountry)}");
            
                return BadRequest(ModelState);
            }

            try
            {
                var country = _mapper.Map<Country>(countryDTO);

                await _unitOfWork.Countries.AddAsync(country);

                await _unitOfWork.SaveAsync();

                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(CreateCountry)} method");

                return StatusCode(500, "Internal Server Error. Please Try Later");
            }
        }

        [Authorize(Policy = "ReqiredUser")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDto countryDto)
        {
            _logger.LogInformation("Try to update country");

            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid attempt in {nameof(UpdateCountry)} method.");
                return BadRequest(ModelState);
            }

            try
            {
                var country = await _unitOfWork.Countries.GetAsync(h => h.Id == id);

                if (country == null)
                {
                    return BadRequest("country id is not valid");
                }

                _mapper.Map(countryDto, country);

                _unitOfWork.Countries.Update(country);

                await _unitOfWork.SaveAsync();

                return Accepted($"Update country with id={id}  was successfuly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(UpdateCountry)} method");
                return Problem($"Something went wrong in the {nameof(UpdateCountry)}", statusCode: 500);
            }
        }

        [Authorize(Policy = "ReqiredUser")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            _logger.LogInformation("Try to delete a country");

            if (id < 1)
            {
                _logger.LogError($"id is not valid in the {DeleteCountry} method");

                return BadRequest("id is not valid");
            }

            try
            {
                var country = await _unitOfWork.Countries.GetAsync(h => h.Id == id);

                if (country == null)
                {
                    _logger.LogError($"submit request to delete country fail because id not valid in the {DeleteCountry} method");

                    return BadRequest("country id not find to delete");
                }

                await _unitOfWork.Countries.RemoveAsync(id);

                await _unitOfWork.SaveAsync();

                return Ok($"Country with id: {id} was delete successfuly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(DeleteCountry)} method");
                return Problem($"Something went wrong in the {nameof(DeleteCountry)}", statusCode: 500);
            }

        }
    }
}
