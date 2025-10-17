using DevNotesApi.Models;

namespace DevNotesApi.Services.Interfaces
{
    public interface INoteService
    {
        Task<IEnumerable<Note>> GetNotesByUserAsync(int folderId, string userId);
        Task<Note?> GetNoteByIdAsync(int id, string userId);
        Task<Note> CreateNoteAsync(Note note, string userId);
        Task<Note?> UpdateNoteAsync(Note note, string userId);
        Task<Note?> DeleteNoteAsync(Note note, string userId);
    }
}
