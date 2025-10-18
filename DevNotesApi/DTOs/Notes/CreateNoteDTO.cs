using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs.Notes
{
    public class CreateNoteDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(250, ErrorMessage = "Title cannot exceed 250 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(10000, ErrorMessage = "Context is too long (max 10,000 characters)")]
        public string Context { get; set; } = string.Empty;

        [StringLength(10000, ErrorMessage = "Code snippet is too long (max 10,000 characters)")]
        public string? CodeSnippet { get; set; }

        /// <summary>
        /// Folder assignment
        /// </summary>
        public int? FolderId { get; set; }

        /// <summary>
        /// Images to attach when creating the note
        /// </summary>
        public List<CreateNoteImageDTO> Images { get; set; } = new List<CreateNoteImageDTO>();
    }
}
