using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs.Notes
{
    public class CreateNoteImageDTO
    {
        [Required(ErrorMessage = "ImagePath is required")]
        public string ImagePath { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
