using DevNotesApi.Data;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevNotesApi.Services.Implementations
{
    /// <summary>
    /// Handles logic for sharing folders between users.
    /// </summary>
    public class SharedFolderService : ISharedFolderService
    {
        private readonly ApplicationDbContext _context;

        public SharedFolderService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all folders shared with or by the user.
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
        /// </summary>
        public async Task<SharedFolder?> ShareFolderAsync(int folderId, string senderId, string receiverId)
        {
            // Prevent self-sharing
            if (senderId == receiverId)
                return null;

            // Validate that the sender owns the folder
            var folder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == senderId);

            if (folder == null)
                return null;

            // Validate that the receiver actually exists
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);
            if (!receiverExists)
                return null;

            // Prevent duplicate sharing between same sender, receiver, and folder
            bool alreadyShared = await _context.SharedFolders
                .AnyAsync(sf =>
                    sf.FolderId == folderId &&
                    sf.SenderId == senderId &&
                    sf.ReceiverId == receiverId);

            if (alreadyShared)
                return null;

            // Create and save the shared folder record
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
        /// Retrieve a specific shared folder either sender or receiver can access it.
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
        /// Remove a shared folder (either sender or receiver can remove access).
        /// </summary>
        public async Task<SharedFolder?> RemoveSharedFolderAsync(SharedFolder sharedFolder, string userId)
        {
            // Only the sender or receiver can remove a shared folder
            if (sharedFolder.SenderId != userId && sharedFolder.ReceiverId != userId)
                return null;

            _context.SharedFolders.Remove(sharedFolder);
            await _context.SaveChangesAsync();

            return sharedFolder;
        }
    }
}
