using DevNotesApi.DTOs;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevNotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        /// <summary>
        /// Get all folders for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.Identity?.Name; // assumes authentication via cookies
            if (userId == null) return Unauthorized();

            var folders = await _folderService.GetFoldersByUserAsync(userId);

            // Map to DTO
            var folderDtos = folders.Select(f => new FolderDTO
            {
                Id = f.Id,
                Name = f.Name,
                ParentFolderId = f.ParentFolderId,
                ParentFolderName = f.ParentFolder?.Name,
                UserId = f.UserId,
                SubFolders = new List<FolderDTO>(), // optionally map nested subfolders
                Notes = new List<NoteDTO>() // optionally map notes
            }).ToList();

            return Ok(folderDtos);
        }

        /// <summary>
        /// Get a folder by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var folder = await _folderService.GetFolderByIdAsync(id, userId);
            if (folder == null) return NotFound();

            var folderDto = new FolderDTO
            {
                Id = folder.Id,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId,
                ParentFolderName = folder.ParentFolder?.Name,
                UserId = folder.UserId,
                SubFolders = new List<FolderDTO>(),
                Notes = new List<NoteDTO>()
            };

            return Ok(folderDto);
        }

        /// <summary>
        /// Create a new folder
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFolderDTO createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var folder = new Folder
            {
                Name = createDto.Name,
                ParentFolderId = createDto.ParentFolderId
            };

            var createdFolder = await _folderService.CreateFolderAsync(folder, userId);
            if (createdFolder == null) return BadRequest("Failed to create folder.");

            var folderDto = new FolderDTO
            {
                Id = createdFolder.Id,
                Name = createdFolder.Name,
                ParentFolderId = createdFolder.ParentFolderId,
                ParentFolderName = createdFolder.ParentFolder?.Name,
                UserId = createdFolder.UserId
            };

            return Ok(folderDto);
        }

        /// <summary>
        /// Update a folder
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateFolderDTO updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var folder = new Folder
            {
                Id = updateDto.Id,
                Name = updateDto.Name,
                ParentFolderId = updateDto.ParentFolderId
            };

            var updatedFolder = await _folderService.UpdateFolderAsync(folder, userId);
            if (updatedFolder == null) return BadRequest("Failed to update folder.");

            var folderDto = new FolderDTO
            {
                Id = updatedFolder.Id,
                Name = updatedFolder.Name,
                ParentFolderId = updatedFolder.ParentFolderId,
                ParentFolderName = updatedFolder.ParentFolder?.Name,
                UserId = updatedFolder.UserId
            };

            return Ok(folderDto);
        }

        /// <summary>
        /// Delete a folder
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var folder = await _folderService.GetFolderByIdAsync(id, userId);
            if (folder == null) return NotFound();

            var deletedFolder = await _folderService.DeleteFolderAsync(folder, userId);
            if (deletedFolder == null) return BadRequest("Failed to delete folder.");

            return Ok($"Folder {deletedFolder.Name} deleted successfully.");
        }
    }
}
