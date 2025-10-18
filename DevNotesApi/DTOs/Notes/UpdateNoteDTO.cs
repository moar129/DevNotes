using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs.Notes
{
    public class UpdateNoteDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(250, ErrorMessage = "Title cannot exceed 250 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(10000, ErrorMessage = "Context is too long (max 10,000 characters)")]
        public string? Context { get; set; }

        [StringLength(10000, ErrorMessage = "Code snippet is too long (max 10,000 characters)")]
        public string? CodeSnippet { get; set; }

        // Allow moving note to a different folder
        public int? FolderId { get; set; }
    }
}
