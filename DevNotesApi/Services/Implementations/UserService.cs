using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevNotes.Services.Implementations
{
    /// <summary>
    /// Handles all user-related operations using ASP.NET Core Identity.
    /// Provides methods for CRUD operations and authentication.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ========================== READ METHODS =================================
        /// <summary>
        /// Get a user by their unique ID.
        /// Returns null if not found.
        /// </summary>
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Get a user by their email address.
        /// Returns null if not found.
        /// </summary>
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Get all users in the system.
        /// </summary>
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        // ========================== WRITE METHODS (CREATE / UPDATE / DELETE) =================================
        /// <summary>
        /// Create a new user with the specified password.
        /// Validates that email and username are unique.
        /// </summary>
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            // Validate email uniqueness
            var existingEmailUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingEmailUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Email is already in use."
                });
            }

            // Validate username uniqueness
            var existingUserName = await _userManager.FindByNameAsync(user.UserName);
            if (existingUserName != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Username is already taken."
                });
            }

            return await _userManager.CreateAsync(user, password);
        }

        /// <summary>
        /// Update an existing user's details.
        /// Returns IdentityResult indicating success or failure.
        /// </summary>
        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Delete a user by their unique ID.
        /// Returns IdentityResult indicating success or failure.
        /// </summary>
        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = $"User with ID '{userId}' not found."
                });
            }

            return await _userManager.DeleteAsync(user);
        }

        // ======================== AUTHENTICATION METHODS ===================================
        /// <summary>
        /// Sign in a user using their email and password.
        /// Returns SignInResult indicating success or failure.
        /// </summary>
        public async Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return SignInResult.Failed;

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(
                user,
                password,
                rememberMe,
                lockoutOnFailure: false
            );
        }

        /// <summary>
        /// Sign out the currently authenticated user.
        /// </summary>
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
