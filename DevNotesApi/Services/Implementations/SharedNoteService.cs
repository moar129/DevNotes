using DevNotesApi.Data;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevNotesApi.Services.Implementations
{
    /// <summary>
    /// Handles logic for sharing notes between users.
    /// </summary>
    public class SharedNoteService : ISharedNoteService
    {
        private readonly ApplicationDbContext _context;

        public SharedNoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all notes shared with or by the user.
        /// </summary>
        public async Task<IEnumerable<SharedNote>> GetSharedNotesForUserAsync(string userId)
        {
            return await _context.SharedNotes
                .Include(sn => sn.Note)
                .Include(sn => sn.FromUser)
                .Include(sn => sn.ToUser)
                .Where(sn => sn.FromUserId == userId || sn.ToUserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Share a note from one user to another.
        /// </summary>
        public async Task<SharedNote?> ShareNoteAsync(int noteId, string fromUserId, string toUserId)
        {
            // Prevent self-sharing
            if (fromUserId == toUserId)
                return null;

            // Validate that the sender owns the note
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == fromUserId);

            if (note == null)
                return null;

            // Ensure receiver exists
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == toUserId);
            if (!receiverExists)
                return null;

            // Prevent duplicate sharing (same sender, receiver, and note)
            bool alreadyShared = await _context.SharedNotes
                .AnyAsync(sn =>
                    sn.NoteId == noteId &&
                    sn.FromUserId == fromUserId &&
                    sn.ToUserId == toUserId);

            if (alreadyShared)
                return null;

            // Create and save shared note
            var sharedNote = new SharedNote
            {
                NoteId = noteId,
                FromUserId = fromUserId,
                ToUserId = toUserId,
                SharedAt = DateTime.Now
            };

            _context.SharedNotes.Add(sharedNote);
            await _context.SaveChangesAsync();

            return sharedNote;
        }

        /// <summary>
        /// Retrieve a specific shared note (sender or receiver can access).
        /// </summary>
        public async Task<SharedNote?> GetSharedNoteAsync(int sharedNoteId, string userId)
        {
            return await _context.SharedNotes
                .Include(sn => sn.Note)
                .Include(sn => sn.FromUser)
                .Include(sn => sn.ToUser)
                .FirstOrDefaultAsync(sn =>
                    sn.Id == sharedNoteId &&
                    (sn.FromUserId == userId || sn.ToUserId == userId));
        }

        /// <summary>
        /// Remove a shared note (only sender or receiver can remove it).
        /// </summary>
        public async Task<SharedNote?> RemoveSharedNoteAsync(SharedNote sharedNote, string userId)
        {
            // Ensure only sender or receiver can remove
            if (sharedNote.FromUserId != userId && sharedNote.ToUserId != userId)
                return null;

            _context.SharedNotes.Remove(sharedNote);
            await _context.SaveChangesAsync();

            return sharedNote;
        }
    }
}
