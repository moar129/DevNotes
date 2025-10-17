using DevNotesApi.Data;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevNotesApi.Services.Implementations
{
    /// <summary>
    /// Service for managing notes within folders.
    /// </summary>
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Constructor injecting the database context.
        /// </summary>
        /// <param name="context"></param>
        public NoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all notes owned by a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Note>> GetNotesByUserAsync(int folderId, string userId)
        {
            // Get all notes in a specific folder owned by the user
            return await _context.Notes
                .Include(n => n.Images)                     // Load associated images
                .Where(n => n.UserId == userId && n.FolderId == folderId) // Filter by user and folder
                .ToListAsync();
        }

        /// <summary>
        /// Get a note by its unique ID and user ownership.
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Note?> GetNoteByIdAsync(int noteId, string userId)
        {
            // Build a query to find the note with matching Id and UserId.
            return await _context.Notes
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);
        }

        /// <summary>
        /// Create a new note in the database.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Note?> CreateNoteAsync(Note note, string userId)
        {
            // Assign ownership
            note.UserId = userId;

            // Validate folder existence if FolderId is provided
            if (note.FolderId.HasValue)
            {
                var folderExists = await _context.Folders
                    .AnyAsync(f => f.Id == note.FolderId.Value && f.UserId == userId);

                if (!folderExists)
                {
                    return null;
                }
            }

            // Add note to database
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return note;
        }

        /// <summary>
        /// Update an existing note.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        // Do NOT overwrite images here — images are handled separately
        public async Task<Note?> UpdateNoteAsync(Note note, string userId)
        {
            // Retrieve the existing note to update
            var existingNote = await _context.Notes
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == note.Id && n.UserId == userId);

            if (existingNote == null)
                return null;

            // Update core fields (text, title, code snippet, folder)
            existingNote.Title = note.Title;
            existingNote.Context = note.Context;
            existingNote.CodeSnippet = note.CodeSnippet;
            existingNote.FolderId = note.FolderId;

            // Save changes to the database
            await _context.SaveChangesAsync();
            return existingNote;
        }

        /// <summary>
        /// Delete a note by its ID.
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Note?> DeleteNoteAsync(Note note, string userId)
        {
            // Ensure the note belongs to the user
            if (note.UserId != userId)
                return null;

            // Mark note for deletion
            _context.Notes.Remove(note);

            // Save changes to database
            await _context.SaveChangesAsync();

            return note;
        }

        /// <summary>
        /// Add an image to a note.
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="image"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<NoteImage?> AddImageToNoteAsync(int noteId, NoteImage image, string userId)
        {
            // Find the note and ensure it belongs to the user
            var note = await _context.Notes
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

            if (note == null)
                return null;

            // Associate the image with the note
            image.NoteId = noteId;
            _context.NoteImages.Add(image);
            await _context.SaveChangesAsync();

            return image;
        }

        /// <summary>
        /// Remove an image from a note.
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="imageId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveImageFromNoteAsync(int noteId, int imageId, string userId)
        {
            // Find the image and ensure it belongs to the user's note
            var image = await _context.NoteImages
                .Include(i => i.Note)
                .FirstOrDefaultAsync(i => i.Id == imageId && i.NoteId == noteId && i.Note.UserId == userId);

            if (image == null)
                return false;

            // Remove the image
            _context.NoteImages.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
