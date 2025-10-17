using DevNotesApi.Models;
using Microsoft.AspNetCore.Identity;

namespace DevNotesApi.Services.Interfaces
{
    public interface IUserService
    {
        // --- Read operations ---
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();

        // --- Write operations (wrapped Identity methods) ---
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> DeleteUserAsync(string userId);

        // --- Authentication ---
        Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe);
        Task SignOutAsync();
    }
}
