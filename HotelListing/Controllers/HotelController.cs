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
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork,ILogger<HotelController> logger,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels =await _unitOfWork.Hotels.GetAllAsync(includes: new List<string> { "Country" }
                    ,orderBy:o=>o.OrderBy(r=>r.Name)
                    );
                var result = _mapper.Map<IEnumerable<HotelDTO>>(hotels);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(GetHotels)} method");
                return StatusCode(500, "Internal Server Error. Please Try Later");
            }
        }

        [HttpGet("{id}",Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                var hotels = await _unitOfWork.Hotels.GetAsync(h=>h.Id==id,
                    includes: new List<string> { "Country" });
                var result = _mapper.Map<HotelDTO>(hotels);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(GetHotel)} method");
                return StatusCode(500, "Internal Server Error. Please Try Later");
            }
        }

        [Authorize(Policy = "ReqiredUser")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHotel([FromBody]CreateHotelDTO hotelDTO)
        {
            _logger.LogInformation("Try to create new hotel");

            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid attempt in {nameof(CreateHotel)} method.");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = _mapper.Map<Hotel>(hotelDTO);
                await _unitOfWork.Hotels.AddAsync(hotel);
                await _unitOfWork.SaveAsync();

                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(CreateHotel)} method");
                return Problem($"Something went wrong in the {nameof(CreateHotel)}", statusCode: 500);
            }
        }

        [Authorize(Policy = "ReqiredUser")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateHotel(int id,[FromBody] UpdateHotelDto hotelDTO)
        {
            _logger.LogInformation("Try to update hotel");

            if (!ModelState.IsValid || id<1)
            {
                _logger.LogError($"Invalid attempt in {nameof(CreateHotel)} method.");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.GetAsync(h => h.Id == id);

                if (hotel==null)
                {
                    return BadRequest("Hotel id is not valid");
                }

                _mapper.Map(hotelDTO, hotel);

                _unitOfWork.Hotels.Update(hotel);

                await _unitOfWork.SaveAsync();

                return Accepted($"Update hotel with id={id}  was successfuly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(UpdateHotel)} method");
                return Problem($"Something went wrong in the {nameof(UpdateHotel)}", statusCode: 500);
            }
        }


        [Authorize(Policy = "ReqiredUser")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            _logger.LogInformation("Try to delete a hotel");

            if (id<1)
            {
                _logger.LogError($"id is not valid in the {DeleteHotel} method");
                
                return BadRequest("id is not valid");
            }

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(DeleteHotel)} method");
                return Problem($"Something went wrong in the {nameof(DeleteHotel)}", statusCode: 500);
            }
            
        }
    }
}
