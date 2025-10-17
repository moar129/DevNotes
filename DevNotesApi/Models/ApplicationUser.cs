using Microsoft.AspNetCore.Identity;

namespace DevNotesApi.Models
{
    public class ApplicationUser: IdentityUser
    {
        public ICollection<Folder> Folders { get; set; }
        public ICollection<Note> Notes { get; set; }
        public ApplicationUser() 
        {
            Folders = new HashSet<Folder>();
            Notes = new HashSet<Note>();
        }
    }
}
