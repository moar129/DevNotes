namespace DevNotesApi.DTOs.Notes
{
    /// <summary>
    /// Data Transfer Object for Note Images.
    /// </summary>
    public class NoteImageDTO
    {
        public int Id { get; set; }

        /// <summary>
        /// Path to the image (matches NoteImage.ImagePath)
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Optional description for the image
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Associated note ID
        /// </summary>
        public int NoteId { get; set; }
    }
}
