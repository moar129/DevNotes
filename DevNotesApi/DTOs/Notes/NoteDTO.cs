using DevNotesApi.DTOs.Notes;

namespace DevNotesApi.DTOs
{
    /// <summary>
    /// Data Transfer Object for Notes.
    /// Used to send and receive note data in API requests/responses.
    /// </summary>
    public class NoteDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public string? CodeSnippet { get; set; }

        public int? FolderId { get; set; }
        public string? FolderName { get; set; }

        public List<NoteImageDTO> Images { get; set; } = new List<NoteImageDTO>();

        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
