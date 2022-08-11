using AutoMapper;
using HotelListing.Data.Interfaces;
using HotelListing.Models.DTOs;
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

        [HttpGet("{id}")]
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
    }
}
