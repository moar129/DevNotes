using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DevNotesApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Folders and notes owned by the user
        public ICollection<Folder> Folders { get; set; } = new HashSet<Folder>();
        public ICollection<Note> Notes { get; set; } = new HashSet<Note>();

        // Shared folders sent and received
        public ICollection<SharedFolder> SentFolders { get; set; } = new HashSet<SharedFolder>();
        public ICollection<SharedFolder> ReceivedFolders { get; set; } = new HashSet<SharedFolder>();

        // Shared notes sent and received
        public ICollection<SharedNote> SentNotes { get; set; } = new HashSet<SharedNote>();
        public ICollection<SharedNote> ReceivedNotes { get; set; } = new HashSet<SharedNote>();
    }
}
