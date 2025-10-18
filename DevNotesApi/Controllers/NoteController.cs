using DevNotesApi.DTOs;
using DevNotesApi.DTOs.Notes;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevNotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        /// <summary>
        /// Get all notes in a folder for the current user
        /// </summary>
        [HttpGet("folder/{folderId}")]
        public async Task<IActionResult> GetNotesByFolder(int folderId)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var notes = await _noteService.GetNotesByUserAsync(folderId, userId);

            var noteDtos = notes.Select(n => new NoteDTO
            {
                Id = n.Id,
                Title = n.Title,
                Context = n.Context,
                CodeSnippet = n.CodeSnippet,
                FolderId = n.FolderId,
                FolderName = n.Folder?.Name,
                UserId = n.UserId,
                Images = n.Images.Select(i => new NoteImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    Description = i.Description,
                    NoteId = i.NoteId
                }).ToList(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }).ToList();

            return Ok(noteDtos);
        }

        /// <summary>
        /// Get a single note by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteById(int id)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var note = await _noteService.GetNoteByIdAsync(id, userId);
            if (note == null) return NotFound();

            var noteDto = new NoteDTO
            {
                Id = note.Id,
                Title = note.Title,
                Context = note.Context,
                CodeSnippet = note.CodeSnippet,
                FolderId = note.FolderId,
                FolderName = note.Folder?.Name,
                UserId = note.UserId,
                Images = note.Images.Select(i => new NoteImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    Description = i.Description,
                    NoteId = i.NoteId
                }).ToList(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return Ok(noteDto);
        }

        /// <summary>
        /// Create a new note
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNoteDTO createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var note = new Note
            {
                Title = createDto.Title,
                Context = createDto.Context,
                CodeSnippet = createDto.CodeSnippet,
                FolderId = createDto.FolderId
            };

            var createdNote = await _noteService.CreateNoteAsync(note, userId);
            if (createdNote == null) return BadRequest("Failed to create note.");

            // Add images if any
            foreach (var imgDto in createDto.Images)
            {
                await _noteService.AddImageToNoteAsync(createdNote.Id, new NoteImage
                {
                    ImagePath = imgDto.ImagePath,
                    Description = imgDto.Description
                }, userId);
            }

            var noteDto = new NoteDTO
            {
                Id = createdNote.Id,
                Title = createdNote.Title,
                Context = createdNote.Context,
                CodeSnippet = createdNote.CodeSnippet,
                FolderId = createdNote.FolderId,
                FolderName = createdNote.Folder?.Name,
                UserId = createdNote.UserId,
                Images = createdNote.Images.Select(i => new NoteImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    Description = i.Description,
                    NoteId = i.NoteId
                }).ToList(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return Ok(noteDto);
        }

        /// <summary>
        /// Update an existing note
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateNoteDTO updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var note = new Note
            {
                Id = updateDto.Id,
                Title = updateDto.Title,
                Context = updateDto.Context,
                CodeSnippet = updateDto.CodeSnippet,
                FolderId = updateDto.FolderId
            };

            var updatedNote = await _noteService.UpdateNoteAsync(note, userId);
            if (updatedNote == null) return BadRequest("Failed to update note.");

            var noteDto = new NoteDTO
            {
                Id = updatedNote.Id,
                Title = updatedNote.Title,
                Context = updatedNote.Context,
                CodeSnippet = updatedNote.CodeSnippet,
                FolderId = updatedNote.FolderId,
                FolderName = updatedNote.Folder?.Name,
                UserId = updatedNote.UserId,
                Images = updatedNote.Images.Select(i => new NoteImageDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    Description = i.Description,
                    NoteId = i.NoteId
                }).ToList(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return Ok(noteDto);
        }

        /// <summary>
        /// Delete a note
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var note = await _noteService.GetNoteByIdAsync(id, userId);
            if (note == null) return NotFound();

            var deletedNote = await _noteService.DeleteNoteAsync(note, userId);
            if (deletedNote == null) return BadRequest("Failed to delete note.");

            return Ok($"Note '{deletedNote.Title}' deleted successfully.");
        }
    }
}
