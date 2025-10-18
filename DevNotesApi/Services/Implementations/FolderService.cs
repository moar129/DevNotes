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
    public class FolderService : IFolderService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor injecting the database context.
        /// </summary>
        public FolderService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all folders owned by a specific user.
        /// </summary>
        public async Task<IEnumerable<Folder>> GetFoldersByUserAsync(string userId)
        {
            var query = _context.Folders
                .Include(f => f.SubFolders)
                .Include(f => f.Notes)
                .Where(f => f.UserId == userId);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get a folder by its ID and user ownership.
        /// </summary>
        public async Task<Folder?> GetFolderByIdAsync(int id, string userId)
        {
            var folder = await _context.Folders
                .Include(f => f.SubFolders)
                .Include(f => f.Notes)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            return folder;
        }

        /// <summary>
        /// Create a new folder for a user, validating name and parent folder.
        /// </summary>
        public async Task<Folder?> CreateFolderAsync(Folder folder, string userId)
        {
            folder.UserId = userId;

            // Validate parent folder exists (if specified)
            if (folder.ParentFolderId.HasValue)
            {
                bool parentExists = await _context.Folders
                    .AnyAsync(f => f.Id == folder.ParentFolderId.Value && f.UserId == userId);

                if (!parentExists)
                    return null; // Parent folder invalid
            }

            // Check for duplicate folder name within same parent
            var existingFolder = await _context.Folders
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId &&
                    f.ParentFolderId == folder.ParentFolderId &&
                    f.Name == folder.Name);

            if (existingFolder != null)
                return null; // Duplicate name

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();

            return folder;
        }

        /// <summary>
        /// Update an existing folder's name or parent.
        /// </summary>
        public async Task<Folder?> UpdateFolderAsync(Folder folder, string userId)
        {
            var existingFolder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folder.Id && f.UserId == userId);

            if (existingFolder == null)
                return null;

            // Validate parent folder exists (if specified)
            if (folder.ParentFolderId.HasValue)
            {
                bool parentExists = await _context.Folders
                    .AnyAsync(f => f.Id == folder.ParentFolderId.Value && f.UserId == userId && f.Id != folder.Id);

                if (!parentExists)
                    return null; // Invalid parent
            }

            // Prevent duplicate folder name in the same parent
            bool duplicateName = await _context.Folders
                .AnyAsync(f => f.UserId == userId &&
                               f.ParentFolderId == folder.ParentFolderId &&
                               f.Name == folder.Name &&
                               f.Id != folder.Id);

            if (duplicateName)
                return null;

            existingFolder.Name = folder.Name;
            existingFolder.ParentFolderId = folder.ParentFolderId;

            await _context.SaveChangesAsync();
            return existingFolder;
        }

        /// <summary>
        /// Recursively deletes a folder and all its notes, images, and subfolders.
        /// </summary>
        public async Task<Folder?> DeleteFolderAsync(Folder folder, string userId)
        {
            var existingFolder = await _context.Folders
                .Include(f => f.Notes)
                    .ThenInclude(n => n.Images)
                .Include(f => f.SubFolders)
                .FirstOrDefaultAsync(f => f.Id == folder.Id && f.UserId == userId);

            if (existingFolder == null)
                return null;

            // --- Step 1: Delete notes and images ---
            foreach (var note in existingFolder.Notes.ToList())
            {
                _context.NoteImages.RemoveRange(note.Images);
                _context.Notes.Remove(note);
            }

            // --- Step 2: Recursively delete subfolders ---
            foreach (var subFolder in existingFolder.SubFolders.ToList())
            {
                await DeleteFolderAsync(subFolder, userId);
            }

            // --- Step 3: Delete the folder itself ---
            _context.Folders.Remove(existingFolder);

            // Save all changes once
            await _context.SaveChangesAsync();

            return existingFolder;
        }
    }
}
