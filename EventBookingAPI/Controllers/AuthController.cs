using Microsoft.AspNetCore.Mvc;
using EventBookingAPI.Services.Interfaces;
using EventBookingAPI.Helper.Exceptions;
using EventBookingAPI.Models.DTOs;
using Microsoft.Extensions.Localization;
namespace EventBookingAPI.Controllers
{
  

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AuthController(IAuthService authService, IStringLocalizer<SharedResource> localizer)
        {
            _authService = authService;
            _localizer = localizer;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (ApiException ex)
            {
                return BadRequest(_localizer["EmailAlreadyExists"]);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (ApiException ex)
            {
                return Unauthorized(_localizer["InvalidCredentials"].Value);
            }
        }
    }
}
