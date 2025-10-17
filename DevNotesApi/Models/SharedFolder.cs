using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevNotesApi.Models
{
    public class SharedFolder
    {
        public int Id { get; set; }

        // Folder FK
        [Required]
        public int FolderId { get; set; }
        public Folder? Folder { get; set; }

        // Sender FK
        [Required]
        public string SenderId { get; set; } = null!;
        public ApplicationUser? Sender { get; set; }

        // Receiver FK
        [Required]
        public string ReceiverId { get; set; } = null!;
        public ApplicationUser? Receiver { get; set; }

        // Timestamp
        public DateTime SharedAt { get; set; } = DateTime.UtcNow;
    }
}
