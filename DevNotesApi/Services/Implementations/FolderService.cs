using DevNotesApi.Data;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevNotesApi.Services.Implementations
{
    /// <summary>
    /// Service for managing folders and their hierarchy (subfolders, notes, ownership).
    /// Handles CRUD operations for folders.
    /// </summary>
    public class FolderService: IFolderService
    {
        // Database context for accessing folder data.
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Constructor injecting the database context.
        /// </summary>
        /// <param name="context"></param>
        public FolderService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all folders owned by a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Folder>> GetFoldersByUserAsync(string userId)
        {
            // Build a query to get all folders for the specified user.
            var query = _context.Folders
                .Include(f => f.SubFolders)  // subfolders inside each folder
                .Include(f => f.Notes)       // notes inside each folder
                .Where(f => f.UserId == userId); // only folders owned by this user

            // Execute the query asynchronously and return a List<Folder>
            var folders = await query.ToListAsync();
            return folders;
        }
        /// <summary>
        /// Get a folder by its unique ID by its ID and user ownership.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Folder?> GetFolderByIdAsync(int id, string userId)
        {
            // Build a query to find the folder with matching Id and UserId.
            // Using FirstOrDefaultAsync will return null if not found.
            var folder = await _context.Folders
                .Include(f => f.SubFolders)
                .Include(f => f.Notes)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            return folder;
        }

        /// <summary>
        /// Create a new folder in the database.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Folder?> CreateFolderAsync(Folder folder, string userId)
        {
            // Assign the folder to the current user
            folder.UserId = userId;

            // Check for duplicate folder name within the same parent folder
            var existingFolder = await _context.Folders
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId &&                          // Only consider folders owned by this user
                    f.ParentFolderId == folder.ParentFolderId &&  // Must be in the same parent folder
                    f.Name == folder.Name                          // Folder name must match
                );

            if (existingFolder != null)
            {
                // Folder already exists in this parent folder
                // Return null or throw an exception depending on how you want to handle it
                return null;
            }

            // Add the folder to the database
            _context.Folders.Add(folder);

            // Save changes to persist it
            await _context.SaveChangesAsync();

            // Return the newly created folder
            return folder;
        }

        /// <summary>
        /// Update an existing folder's details.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Folder?> UpdateFolderAsync(Folder folder, string userId)
        {
            // Find the existing folder by its ID and user ownership.
            var existingFolder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folder.Id && f.UserId == userId);

            // If the folder doesn't exist, return null.
            if (existingFolder == null)
                return null;

            // Update the folder's properties.
            existingFolder.Name = folder.Name;
            existingFolder.ParentFolderId = folder.ParentFolderId;

            // Save changes to the database.
            await _context.SaveChangesAsync();

            // Return the updated folder.
            return existingFolder;
        }

        /// <summary>
        /// Delete a folder from the database.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Folder?> DeleteFolderAsync(Folder folder, string userId)
        {
            // Find the existing folder by its ID and user ownership.
            var existingFolder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folder.Id && f.UserId == userId);

            // If the folder doesn't exist, return null.
            if (existingFolder == null)
                return null;

            // Remove the folder from the DbSet.
            _context.Folders.Remove(existingFolder);

            // Save changes to the database.
            await _context.SaveChangesAsync();

            // Return the deleted folder.
            return existingFolder;
        }
    }
}
