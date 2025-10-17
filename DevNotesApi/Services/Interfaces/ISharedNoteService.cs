using DevNotesApi.Models;

namespace DevNotesApi.Services.Interfaces
{
    public interface ISharedNoteService
    {
        Task<IEnumerable<SharedNote>> GetSharedNotesForUserAsync(string userId);
        Task<SharedNote> ShareNoteAsync(int noteId, string fromUserId, string toUserId);
        Task<SharedNote?> RemoveSharedAccessAsync(Note note, string receiverId);

    }
}
