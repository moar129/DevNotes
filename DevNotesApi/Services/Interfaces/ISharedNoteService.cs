using DevNotesApi.Models;

namespace DevNotesApi.Services.Interfaces
{
    public interface ISharedNoteService
    {
        Task<IEnumerable<SharedNote>> GetSharedNotesForUserAsync(string userId);
        Task<SharedNote?> ShareNoteAsync(int noteId, string fromUserId, string toUserId);
        Task<SharedNote?> GetSharedNoteAsync(int sharedNoteId, string userId);
        Task<SharedNote?> RemoveSharedNoteAsync(SharedNote sharedNote, string userId);

    }
}
