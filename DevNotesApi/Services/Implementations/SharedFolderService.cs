using DevNotesApi.Data;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevNotesApi.Services.Implementations
{
    /// <summary>
    /// Handles logic for sharing folders between users.
    /// Returns null when operations are invalid.
    /// </summary>
    public class SharedFolderService : ISharedFolderService
    {
        private readonly ApplicationDbContext _context;

        public SharedFolderService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all folders shared *with* or *by* the specified user.
        /// </summary>
        public async Task<IEnumerable<SharedFolder>> GetSharedFoldersForUserAsync(string userId)
        {
            return await _context.SharedFolders
                .Include(sf => sf.Folder)
                .Include(sf => sf.Sender)
                .Include(sf => sf.Receiver)
                .Where(sf => sf.SenderId == userId || sf.ReceiverId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Share a folder from one user to another.
        /// Returns null if invalid (self-share, non-existent folder, receiver, or duplicate).
        /// </summary>
        public async Task<SharedFolder?> ShareFolderAsync(int folderId, string senderId, string receiverId)
        {
            // Prevent self-sharing
            if (senderId == receiverId)
                return null;

            // Validate sender owns the folder
            var folder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == senderId);

            if (folder == null)
                return null;

            // Validate receiver exists
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);
            if (!receiverExists)
                return null;

            // Prevent duplicate sharing
            bool alreadyShared = await _context.SharedFolders
                .AnyAsync(sf =>
                    sf.FolderId == folderId &&
                    sf.SenderId == senderId &&
                    sf.ReceiverId == receiverId);

            if (alreadyShared)
                return null;

            // Create the shared folder record
            var sharedFolder = new SharedFolder
            {
                FolderId = folderId,
                SenderId = senderId,
                ReceiverId = receiverId,
                SharedAt = DateTime.UtcNow
            };

            _context.SharedFolders.Add(sharedFolder);
            await _context.SaveChangesAsync();

            return sharedFolder;
        }

        /// <summary>
        /// Get a shared folder by ID.
        /// Only accessible to the sender or receiver.
        /// Returns null if not found or user has no access.
        /// </summary>
        public async Task<SharedFolder?> GetSharedFolderAsync(int sharedFolderId, string userId)
        {
            return await _context.SharedFolders
                .Include(sf => sf.Folder)
                .Include(sf => sf.Sender)
                .Include(sf => sf.Receiver)
                .FirstOrDefaultAsync(sf =>
                    sf.Id == sharedFolderId &&
                    (sf.SenderId == userId || sf.ReceiverId == userId));
        }

        /// <summary>
        /// Remove a shared folder (sender or receiver can remove access).
        /// Returns null if user cannot remove it.
        /// </summary>
        public async Task<SharedFolder?> RemoveSharedFolderAsync(SharedFolder sharedFolder, string userId)
        {
            // Always check ownership even if the entity is passed in
            if (sharedFolder.SenderId != userId && sharedFolder.ReceiverId != userId)
                return null;

            // Optionally, reload from DB to ensure the entity exists
            var existing = await _context.SharedFolders
                .FirstOrDefaultAsync(sf => sf.Id == sharedFolder.Id);

            if (existing == null)
                return null;

            _context.SharedFolders.Remove(existing);
            await _context.SaveChangesAsync();

            return existing;
        }
    }
}
