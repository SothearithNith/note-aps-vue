using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NotesApi.Models;
using NotesApi.Repositories;
using System.Security.Claims;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require JWT
    public class NotesController : ControllerBase
    {
        private readonly NoteRepository _noteRepository;

        public NotesController(NoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Note note)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            note.UserId = userId;

            var id = await _noteRepository.CreateAsync(note);
            return Ok(new { id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var notes = await _noteRepository.GetByUserIdAsync(userId);
            return Ok(notes);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Note note)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            note.Id = id;
            note.UserId = userId;

            var exists = await _noteRepository.GetByIdAsync(id, userId);
            if (exists == null)
                return NotFound("Note not found or you don't have permission.");

            var updated = await _noteRepository.UpdateAsync(note);
            if (!updated)
                return StatusCode(500, "Failed to update note.");

            return NoContent(); // 204, successful update
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var exists = await _noteRepository.GetByIdAsync(id, userId);
            if (exists == null)
                return NotFound("Note not found or you don't have permission.");

            var deleted = await _noteRepository.DeleteAsync(id, userId);
            if (!deleted)
                return StatusCode(500, "Failed to delete note.");

            return NoContent();
        }
    }
}