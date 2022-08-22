using AutoMapper;
using HotelListing.Data.Interfaces;
using HotelListing.Models;
using HotelListing.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        //private readonly SignInManager<ApiUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;
        private readonly ITokenService _tokenService;

        public AccountController(
            //SignInManager<ApiUser> signInManager,
            UserManager<ApiUser> userManager,
            IMapper mapper,
            ILogger<AccountController> logger,
            ITokenService tokenService
            )
        {
            //_signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            _logger.LogInformation($"Register attempt for {userDto.Email}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _userManager.Users.AnyAsync(u => u.Email == userDto.Email))
                    return BadRequest("Email is taken");

                var user=_mapper.Map<ApiUser>(userDto);

                user.UserName = userDto.Email;

                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest("User registration attempt failed.");
                }

                await _userManager.AddToRolesAsync(user, userDto.Roles);

                return Accepted("User registration was successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error accure during call {nameof(Register)} method");
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            _logger.LogInformation($"Login attempt for {loginDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user=await _userManager.Users.SingleOrDefaultAsync(x => x.Email == loginDTO.Email);

            if (user == null) return Unauthorized($"{loginDTO.Email} is not valid");


            
            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result) return Unauthorized("Your password is mistak");

            var token = await _tokenService.CreateTokenAsync(user);

            return Accepted(new
            {
                username = user.UserName,
                token = token
            });

        }
    }
}
