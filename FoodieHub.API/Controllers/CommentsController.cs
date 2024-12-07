using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Comment;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _service;

        public CommentsController(ICommentService service)
        {
            _service = service;
        }

        [HttpGet("recipes/{id}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetByRecipe(int id)
        {
            var result = await _service.GetByRecipe(id);
            return Ok(result);
        }
        [HttpGet("articles/{id}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetByArticle(int id)
        {
            var result = await _service.GetByArticle(id);
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Comment>> Create([FromBody]Comment comment)
        {
            var result = await _service.Create(comment);
            if(result==null) return BadRequest();
            return Ok(result);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(int id,[FromBody] Comment comment)
        {
            bool result = await _service.Edit(id,comment);
            return result? NoContent():BadRequest();
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await _service.Delete(id);
            return result ? NoContent() : BadRequest();
        }
    }
}
