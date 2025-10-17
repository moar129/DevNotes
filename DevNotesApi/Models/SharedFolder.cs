using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.Models
{
    public class SharedFolder
    {
        public int Id { get; set; }

        [Required]
        public int FolderId { get; set; }

        [ForeignKey(nameof(FolderId))]
        public Folder Folder { get; set; } = null!;

        [Required]
        public string SenderId { get; set; } = null!;

        [ForeignKey(nameof(SenderId))]
        public ApplicationUser Sender { get; set; } = null!;

        [Required]
        public string ReceiverId { get; set; } = null!;

        [ForeignKey(nameof(ReceiverId))]
        public ApplicationUser Receiver { get; set; } = null!;

        public DateTime SharedAt { get; set; } = DateTime.UtcNow;
    }
}
