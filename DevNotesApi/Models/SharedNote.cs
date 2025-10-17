using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.Models
{
    public class SharedNote
    {
        public int Id { get; set; }
        public bool CanEdit { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime SharedAt { get; set; } = DateTime.Now;

        // Entity Framework Core will automatically manage these properties
        [Required]
        public int NoteId { get; set; }
        public Note Note { get; set; }

        [Required]
        public string FromUserId { get; set; }
        public ApplicationUser FromUser { get; set; }

        [Required]
        public string ToUserId { get; set; }
        public ApplicationUser ToUser { get; set; }
    }
}
