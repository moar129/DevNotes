using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.DTOs
{
    public class CreateFolderDTO
    {
        [Required(ErrorMessage = "Folder name is required")]
        [StringLength(100, ErrorMessage = "Folder name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        // Optional parent folder
        public int? ParentFolderId { get; set; }
    }
}
