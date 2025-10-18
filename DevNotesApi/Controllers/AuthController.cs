using DevNotesApi.DTOs.Users;
using DevNotesApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevNotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Login a user with email/password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                loginDto.RememberMe
            );

            if (!result.Succeeded)
                return Unauthorized("Invalid email or password.");

            return Ok("Login successful");
        }

        /// <summary>
        /// Logout the current user
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.SignOutAsync();
            return Ok("Logged out successfully");
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new Models.ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var result = await _userService.CreateUserAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully");
        }
    }
}
