using DevNotesApi.DTOs.Shared;
using DevNotesApi.Models;
using DevNotesApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevNotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SharedFolderController : ControllerBase
    {
        private readonly ISharedFolderService _sharedFolderService;

        public SharedFolderController(ISharedFolderService sharedFolderService)
        {
            _sharedFolderService = sharedFolderService;
        }

        /// <summary>
        /// Get all folders shared with or by the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSharedFolders()
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var sharedFolders = await _sharedFolderService.GetSharedFoldersForUserAsync(userId);

            var dtos = sharedFolders.Select(sf => new SharedFolderDTO
            {
                Id = sf.Id,
                FolderId = sf.FolderId,
                FolderName = sf.Folder?.Name ?? string.Empty,
                SenderId = sf.SenderId,
                SenderEmail = sf.Sender?.Email ?? string.Empty,
                ReceiverId = sf.ReceiverId,
                ReceiverEmail = sf.Receiver?.Email ?? string.Empty,
                SharedAt = sf.SharedAt
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Share a folder with another user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ShareFolder([FromBody] CreateSharedFolderDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var senderId = User.Identity?.Name;
            if (senderId == null) return Unauthorized();

            var sharedFolder = await _sharedFolderService.ShareFolderAsync(dto.FolderId, senderId, dto.ReceiverId);
            if (sharedFolder == null) return BadRequest("Failed to share folder.");

            var sharedFolderDto = new SharedFolderDTO
            {
                Id = sharedFolder.Id,
                FolderId = sharedFolder.FolderId,
                FolderName = sharedFolder.Folder?.Name ?? string.Empty,
                SenderId = sharedFolder.SenderId,
                SenderEmail = sharedFolder.Sender?.Email ?? string.Empty,
                ReceiverId = sharedFolder.ReceiverId,
                ReceiverEmail = sharedFolder.Receiver?.Email ?? string.Empty,
                SharedAt = sharedFolder.SharedAt
            };

            return Ok(sharedFolderDto);
        }

        /// <summary>
        /// Remove a shared folder
        /// </summary>
        [HttpDelete("{sharedFolderId}")]
        public async Task<IActionResult> RemoveSharedFolder(int sharedFolderId)
        {
            var userId = User.Identity?.Name;
            if (userId == null) return Unauthorized();

            var sharedFolder = await _sharedFolderService.GetSharedFolderAsync(sharedFolderId, userId);
            if (sharedFolder == null) return NotFound();

            var result = await _sharedFolderService.RemoveSharedFolderAsync(sharedFolder, userId);
            if (result == null) return BadRequest("Failed to remove shared folder.");

            return Ok($"Shared folder with ID {sharedFolderId} removed successfully.");
        }
    }
}
