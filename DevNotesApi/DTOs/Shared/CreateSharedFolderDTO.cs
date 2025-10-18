using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs.Shared
{
    public class CreateSharedFolderDTO
    {
        public int FolderId { get; set; }

        [Required]
        public string ReceiverId { get; set; } = string.Empty;
    }
}
