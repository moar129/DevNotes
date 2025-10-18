using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs.Users
{
    public class RegisterUserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}
