using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs.Shared
{
    public class CreateSharedNoteDTO
    {
        [Required]
        public int NoteId { get; set; }

        [Required]
        public string ToUserId { get; set; } = string.Empty;

        public bool CanEdit { get; set; } = false;
    }
}
