using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs.Users
{
    public class UpdateUserDTO
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}
