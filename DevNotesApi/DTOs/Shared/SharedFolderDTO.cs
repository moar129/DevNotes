namespace DevNotesApi.DTOs.Shared
{
    public class SharedFolderDTO
    {
        public int Id { get; set; }

        // Folder info
        public int FolderId { get; set; }
        public string FolderName { get; set; } = string.Empty;

        // Sender info
        public string SenderId { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;

        // Receiver info
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverEmail { get; set; } = string.Empty;

        // Timestamp
        public DateTime SharedAt { get; set; }
    }
}
