using DevNotesApi.Models;

namespace DevNotesApi.Services.Interfaces
{
    public interface ISharedFolderService
    {
        Task<IEnumerable<SharedFolder>> GetSharedFoldersForUserAsync(string userId);
        Task<SharedFolder?> ShareFolderAsync(int folderId, string senderId, string receiverId);
        Task<SharedFolder?> GetSharedFolderAsync(int sharedFolderId, string userId);
        Task<SharedFolder?> RemoveSharedFolderAsync(SharedFolder sharedFolder, string userId);
    }
}
