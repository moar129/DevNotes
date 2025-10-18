using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.Models
{
    public class NoteImage
    {
        public int Id { get; set; }
        [Required]
        public string ImagePath { get; set; }
        [StringLength(100, ErrorMessage = "Image Description is too long (max 100 characters)")]
        public string? Description { get; set; }

        // Entity Framework Core will automatically manage these properties
        [Required]
        public int NoteId { get; set; }
        public Note? Note { get; set; }
    }
}
