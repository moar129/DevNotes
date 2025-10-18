using DevNotesApi.DTOs.Shared;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevNotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SharedNoteController : ControllerBase
    {
        private readonly ISharedNoteService _sharedNoteService;

        public SharedNoteController(ISharedNoteService sharedNoteService)
        {
            _sharedNoteService = sharedNoteService;
        }

        /// <summary>
        /// Get all notes shared with or by the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSharedNotes()
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var sharedNotes = await _sharedNoteService.GetSharedNotesForUserAsync(userId);

            var dtos = sharedNotes.Select(sn => new SharedNoteDTO
            {
                Id = sn.Id,
                CanEdit = sn.CanEdit,
                NoteId = sn.NoteId,
                NoteTitle = sn.Note?.Title ?? string.Empty,
                FromUserId = sn.FromUserId,
                FromUserEmail = sn.FromUser?.Email ?? string.Empty,
                ToUserId = sn.ToUserId,
                ToUserEmail = sn.ToUser?.Email ?? string.Empty,
                SharedAt = sn.SharedAt
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Share a note with another user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ShareNote([FromBody] CreateSharedNoteDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var fromUserId = User.Identity?.Name;
            if (fromUserId == null) return Unauthorized();

            var sharedNote = await _sharedNoteService.ShareNoteAsync(dto.NoteId, fromUserId, dto.ToUserId);
            if (sharedNote == null) return BadRequest("Failed to share note.");

            sharedNote.CanEdit = dto.CanEdit;

            var sharedNoteDto = new SharedNoteDTO
            {
                Id = sharedNote.Id,
                CanEdit = sharedNote.CanEdit,
                NoteId = sharedNote.NoteId,
                NoteTitle = sharedNote.Note?.Title ?? string.Empty,
                FromUserId = sharedNote.FromUserId,
                FromUserEmail = sharedNote.FromUser?.Email ?? string.Empty,
                ToUserId = sharedNote.ToUserId,
                ToUserEmail = sharedNote.ToUser?.Email ?? string.Empty,
                SharedAt = sharedNote.SharedAt
            };

            return Ok(sharedNoteDto);
        }

        /// <summary>
        /// Remove a shared note
        /// </summary>
        [HttpDelete("{sharedNoteId}")]
        public async Task<IActionResult> RemoveSharedNote(int sharedNoteId)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var sharedNote = await _sharedNoteService.GetSharedNoteAsync(sharedNoteId, userId);
            if (sharedNote == null) return NotFound();

            var result = await _sharedNoteService.RemoveSharedNoteAsync(sharedNote, userId);
            if (result == null) return BadRequest("Failed to remove shared note.");

            return Ok($"Shared note with ID {sharedNoteId} removed successfully.");
        }
    }
}
