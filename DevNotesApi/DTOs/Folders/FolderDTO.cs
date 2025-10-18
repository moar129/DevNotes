namespace DevNotesApi.DTOs
{
    public class FolderDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Optional parent folder info
        public int? ParentFolderId { get; set; }
        public string? ParentFolderName { get; set; }

        // Owner
        public string UserId { get; set; } = string.Empty;

        // Nested subfolders
        public List<FolderDTO> SubFolders { get; set; } = new List<FolderDTO>();

        // Optional: notes in this folder
        public List<NoteDTO> Notes { get; set; } = new List<NoteDTO>();
    }
}
