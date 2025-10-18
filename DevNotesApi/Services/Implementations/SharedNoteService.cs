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

            // Validate sender owns the note
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == fromUserId);
            if (note == null)
                return null;

            // Validate receiver exists
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == toUserId);
            if (!receiverExists)
                return null;

            // Prevent duplicate sharing
            bool alreadyShared = await _context.SharedNotes
                .AnyAsync(sn =>
                    sn.NoteId == noteId &&
                    sn.FromUserId == fromUserId &&
                    sn.ToUserId == toUserId);
            if (alreadyShared)
                return null;

            // Create shared note
            var sharedNote = new SharedNote
            {
                NoteId = noteId,
                FromUserId = fromUserId,
                ToUserId = toUserId,
                SharedAt = DateTime.UtcNow
            };

            _context.SharedNotes.Add(sharedNote);
            await _context.SaveChangesAsync();

            // Optionally load related entities
            await _context.Entry(sharedNote).Reference(sn => sn.Note).LoadAsync();
            await _context.Entry(sharedNote).Reference(sn => sn.FromUser).LoadAsync();
            await _context.Entry(sharedNote).Reference(sn => sn.ToUser).LoadAsync();

            return sharedNote;
        }

        /// <summary>
        /// Retrieve a specific shared note (sender or receiver can access it).
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
            // Ensure entity exists
            var existing = await _context.SharedNotes
                .FirstOrDefaultAsync(sn => sn.Id == sharedNote.Id);

            if (existing == null)
                return null;

            // Only sender or receiver can remove
            if (existing.FromUserId != userId && existing.ToUserId != userId)
                return null;

            _context.SharedNotes.Remove(existing);
            await _context.SaveChangesAsync();

            return existing;
        }
    }
}
