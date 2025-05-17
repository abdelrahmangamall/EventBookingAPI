using EventBookingAPI.Data;
using EventBookingAPI.Models;
using EventBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using EventBookingAPI.Helper;
using EventBookingAPI.Helper.Exceptions;
using EventBookingAPI.Models.DTOs;

namespace EventBookingAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(UserManager<ApplicationUser> userManager, AppDbContext context, JwtHelper jwtHelper)
        {
            _userManager = userManager;
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ApiException("Email already exists");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new ApiException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "User");

            return new AuthResponse
            {
                Token = _jwtHelper.GenerateToken(user),
                UserId = user.Id,
                UserName = user.FullName,
                Email = user.Email
            };
        }

        public async Task<AuthResponse> LoginAsync(Models.DTOs.LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException("Invalid credentials");
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
            {
                throw new ApiException("Invalid credentials");
            }

            return new AuthResponse
            {
                Token = _jwtHelper.GenerateToken(user),
                UserId = user.Id,
                UserName = user.FullName,
                Email = user.Email
            };
        }
    }
}
