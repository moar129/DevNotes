using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.Models
{
    public class Note
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [StringLength(250, ErrorMessage = "Title cannot exceed 250 characters")]
        public string Title { get; set; }
        [StringLength(10000, ErrorMessage = "Context is too long (max 10,000 characters)")]
        public string Context { get; set; }
        [StringLength(10000, ErrorMessage = "Code snippet is too long (max 10,000 characters)")]
        public string? CodeSnippet { get; set; }

        // Entity Framework Core will automatically manage these properties
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int? FolderId { get; set; }
        public Folder Folder { get; set; }

        // Navigation properties
        public ICollection<NoteImage> Images { get; set; }
        public Note()
        {
            Images = new HashSet<NoteImage>();
        }
    }
}
