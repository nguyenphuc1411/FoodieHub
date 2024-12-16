using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _service;

        public ArticlesController(IArticleService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedModel<GetArticleDTO>>> Get([FromQuery] QueryArticleModel query)
        {
            var result = await _service.Get(query);
            return Ok(result);
        }
        [HttpGet("users/{userID}")]
        public async Task<ActionResult<IEnumerable<GetArticleDTO>>> GetOfUser([FromRoute]string userID)
        {
            var result = await _service.GetOfUser(userID);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GetArticleDTO>> GetByID([FromRoute]int id)
        {
            var result = await _service.GetByID(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateArticleDTO articleDTO)
        {
            var result = await _service.Create(articleDTO);
            return result? Ok():BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,UpdateArticleDTO articleDTO)
        {
            if(id!=articleDTO.ArticleID) return BadRequest();
            var result = await _service.Update(articleDTO);
            return result ? NoContent() : BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _service.Delete(id);
            return result? NoContent():BadRequest();
        }
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ArticleByCategory>>> GetByCategory()
        {
            var result = await _service.GetByCategory();
            return Ok(result);
        }

    }
}
