using DevNotesApi.Data;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevNotesApi.Services.Implementations
{
    /// <summary>
    /// Service for managing notes within folders.
    /// Handles CRUD operations and note images.
    /// </summary>
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor injecting the database context.
        /// </summary>
        public NoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all notes in a specific folder for a user.
        /// </summary>
        public async Task<IEnumerable<Note>> GetNotesByUserAsync(int folderId, string userId)
        {
            return await _context.Notes
                .Include(n => n.Images)  // Include associated images
                .Where(n => n.UserId == userId && n.FolderId == folderId)
                .ToListAsync();
        }

        /// <summary>
        /// Get all notes for a user, regardless of folder.
        /// </summary>
        public async Task<IEnumerable<Note>> GetAllNotesByUserAsync(string userId)
        {
            return await _context.Notes
                .Include(n => n.Images)
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Get a single note by its ID and ensure it belongs to the user.
        /// </summary>
        public async Task<Note?> GetNoteByIdAsync(int noteId, string userId)
        {
            return await _context.Notes
                .Include(n => n.Images) // Include images
                .Include(n => n.Folder) // Include folder info if needed
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);
        }

        /// <summary>
        /// Create a new note for a user.
        /// </summary>
        public async Task<Note?> CreateNoteAsync(Note note, string userId)
        {
            // Assign ownership
            note.UserId = userId;

            // Validate folder exists and belongs to the user
            if (note.FolderId.HasValue)
            {
                bool folderExists = await _context.Folders
                    .AnyAsync(f => f.Id == note.FolderId.Value && f.UserId == userId);

                if (!folderExists)
                    return null;
            }

            // Prevent duplicate note titles in the same folder
            bool duplicateTitle = await _context.Notes
                .AnyAsync(n => n.UserId == userId &&
                               n.FolderId == note.FolderId &&
                               n.Title == note.Title);
            if (duplicateTitle)
                return null;

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return note;
        }

        /// <summary>
        /// Update an existing note.
        /// Images are NOT updated here—they are managed separately.
        /// </summary>
        public async Task<Note?> UpdateNoteAsync(Note note, string userId)
        {
            // Find existing note
            var existingNote = await _context.Notes
                .Include(n => n.Images) // Include images for reference
                .FirstOrDefaultAsync(n => n.Id == note.Id && n.UserId == userId);

            if (existingNote == null)
                return null;

            // Optional: Validate folder ownership if changing folder
            if (note.FolderId.HasValue && note.FolderId != existingNote.FolderId)
            {
                bool folderExists = await _context.Folders
                    .AnyAsync(f => f.Id == note.FolderId.Value && f.UserId == userId);

                if (!folderExists)
                    return null;
            }

            // Update properties
            existingNote.Title = note.Title;
            existingNote.Context = note.Context;
            existingNote.CodeSnippet = note.CodeSnippet;
            existingNote.FolderId = note.FolderId;

            await _context.SaveChangesAsync();
            return existingNote;
        }

        /// <summary>
        /// Delete a note and all associated images.
        /// </summary>
        public async Task<Note?> DeleteNoteAsync(Note note, string userId)
        {
            // Check ownership
            if (note.UserId != userId)
                return null;

            // Load images explicitly to remove them
            await _context.Entry(note)
                .Collection(n => n.Images)
                .LoadAsync();

            if (note.Images.Any())
            {
                _context.NoteImages.RemoveRange(note.Images);
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return note;
        }

        /// <summary>
        /// Add an image to a note.
        /// </summary>
        public async Task<NoteImage?> AddImageToNoteAsync(int noteId, NoteImage image, string userId)
        {
            var note = await _context.Notes
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

            if (note == null)
                return null;

            image.NoteId = noteId;
            _context.NoteImages.Add(image);
            await _context.SaveChangesAsync();

            return image;
        }

        /// <summary>
        /// Remove an image from a note.
        /// </summary>
        public async Task<bool> RemoveImageFromNoteAsync(int noteId, int imageId, string userId)
        {
            var image = await _context.NoteImages
                .Include(i => i.Note)
                .FirstOrDefaultAsync(i => i.Id == imageId && i.NoteId == noteId && i.Note.UserId == userId);

            if (image == null)
                return false;

            _context.NoteImages.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
