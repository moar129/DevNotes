namespace DevNotesApi.DTOs.Shared
{
    public class SharedNoteDTO
    {
        public int Id { get; set; }
        public bool CanEdit { get; set; }

        // Note info
        public int NoteId { get; set; }
        public string NoteTitle { get; set; } = string.Empty;

        // Sender info
        public string FromUserId { get; set; } = string.Empty;
        public string FromUserEmail { get; set; } = string.Empty;

        // Receiver info
        public string ToUserId { get; set; } = string.Empty;
        public string ToUserEmail { get; set; } = string.Empty;

        public DateTime SharedAt { get; set; }
    }
}
