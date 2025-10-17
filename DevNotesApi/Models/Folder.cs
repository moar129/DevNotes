using System.ComponentModel.DataAnnotations;

namespace DevNotesApi.Models
{
    public class Folder
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Folder name is required")]
        [StringLength(100, ErrorMessage = "Folder name cannot exceed 100 characters")]
        public string Name { get; set; }

        // Entity Framework Core will automatically manage these properties
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int? ParentFolderId { get; set; }
        public Folder ParentFolder { get; set; }

        // Navigation properties
        public ICollection<Folder> SubFolders { get; set; }
        public ICollection<Note> Notes { get; set; }
        /// <summary>
        /// Constructor to initialize collections.
        /// </summary>
        public Folder()
        {
            SubFolders = new HashSet<Folder>();
            Notes = new HashSet<Note>();
        }
    }
}
