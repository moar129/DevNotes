using DevNotesApi.Models;

namespace DevNotesApi.Services.Interfaces
{
    public interface ISharedFolderService
    {
        Task<IEnumerable<SharedFolder>> GetSharedFoldersForUserAsync(string userId);
        Task<SharedFolder?> ShareFolderAsync(int folderId, string senderId, string receiverId);
        Task<SharedFolder?> RemoveSharedFolderAsync(Folder folder, string receiverId);
        Task<bool> HasAccessToFolderAsync(int folderId, string userId);
    }
}
