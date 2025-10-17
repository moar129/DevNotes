using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.Models
{
    public class NoteImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }

        // Entity Framework Core will automatically manage these properties
        [Required]
        public int NoteId { get; set; }
        public Note Note { get; set; }
    }
}
