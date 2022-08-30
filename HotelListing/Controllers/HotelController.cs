using AutoMapper;
using FluentValidation;
using HotelListing.Data.Interfaces;
using HotelListing.Extensions;
using HotelListing.Models;
using HotelListing.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/{v:apiversion}/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IValidator<CreateHotelDTO> _validator;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IValidator<CreateHotelDTO> validator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _validator = validator;
            _mapper = mapper;
        }


        [HttpGet]
        [ResponseCache(CacheProfileName = "60Cache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels([FromQuery] RequestPagingParams requestParams)
        {

            var hotels = await _unitOfWork.Hotels.GetAllWithPagingAsync(includes: new List<string> { "Country" }
                , orderBy: o => o.OrderBy(r => r.Name),
                request: requestParams
                );
            var result = _mapper.Map<IEnumerable<HotelDTO>>(hotels);

            return Ok(result);
        }


        [HttpGet("{id}", Name = "GetHotel")]
        [ResponseCache(CacheProfileName = "60Cache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            var hotels = await _unitOfWork.Hotels.GetAsync(h => h.Id == id,
                includes: new List<string> { "Country" });
            var result = _mapper.Map<HotelDTO>(hotels);
            return Ok(result);

        }


        [Authorize(Policy = "ReqiredUser")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            var validationResult = await _validator.ValidateAsync(hotelDTO);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);

                _logger.LogError($"Invalid attempt in {nameof(CreateHotel)} method.");
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            //try
            //{
            var hotel = _mapper.Map<Hotel>(hotelDTO);
            await _unitOfWork.Hotels.AddAsync(hotel);
            await _unitOfWork.SaveAsync();

            return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, $"An error accure during call {nameof(CreateHotel)} method");
            //    return Problem($"Something went wrong in the {nameof(CreateHotel)}", statusCode: 500);
            //}
        }


        [Authorize(Policy = "ReqiredUser")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDto hotelDTO)
        {
            _logger.LogInformation("Try to update hotel");

            var validationResult = await _validator.ValidateAsync(hotelDTO);

            if (!validationResult.IsValid || id < 1)
            {
                validationResult.AddToModelState(ModelState);

                _logger.LogError($"Invalid attempt in {nameof(CreateHotel)} method.");
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var hotel = await _unitOfWork.Hotels.GetAsync(h => h.Id == id);

            if (hotel == null)
            {
                return BadRequest("Hotel id is not valid");
            }

            _mapper.Map(hotelDTO, hotel);

            _unitOfWork.Hotels.Update(hotel);

            await _unitOfWork.SaveAsync();

            return Accepted($"Update hotel with id={id}  was successfuly");
        }


        [Authorize(Policy = "ReqiredUser")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            _logger.LogInformation("Try to delete a hotel");

            if (id < 1)
            {
                _logger.LogError($"id is not valid in the {DeleteHotel} method");

                return BadRequest("id is not valid");
            }

            var hotel = await _unitOfWork.Hotels.GetAsync(h => h.Id == id);

            if (hotel == null)
            {
                _logger.LogError($"submit request to delete hotel fail because id not valid in the {DeleteHotel} method");

                return BadRequest("hotel id not find to delete");
            }

            await _unitOfWork.Hotels.RemoveAsync(hotel.Id);

            await _unitOfWork.SaveAsync();

            return Ok($"Hotel with id: {id} was delete successfuly");
        }
    }
}
