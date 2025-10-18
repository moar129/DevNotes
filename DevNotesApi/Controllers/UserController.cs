using DevNotesApi.DTOs.Users;
using DevNotesApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevNotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GetUserByIdAsync(updateDto.Id);
            if (user == null) return NotFound();

            user.Email = updateDto.Email;
            user.UserName = updateDto.UserName;

            var result = await _userService.UpdateUserAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("User updated successfully");
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User deleted successfully");
        }
    }
}
