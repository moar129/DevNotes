using DevNotesApi.Models;

namespace DevNotesApi.Services.Interfaces
{
    public interface IFolderService
    {
        Task<IEnumerable<Folder>> GetFoldersByUserAsync(string userId);
        Task<Folder?> GetFolderByIdAsync(int id, string userId);
        Task<Folder> CreateFolderAsync(Folder folder, string userId);
        Task<Folder?> UpdateFolderAsync(Folder folder, string userId);
        Task<Folder?> DeleteFolderAsync(Folder folder, string userId);
    }
}
