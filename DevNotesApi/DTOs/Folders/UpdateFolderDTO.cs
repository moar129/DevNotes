using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs
{
    public class UpdateFolderDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Folder name is required")]
        [StringLength(100, ErrorMessage = "Folder name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        // Update parent folder
        public int? ParentFolderId { get; set; }
    }
}
