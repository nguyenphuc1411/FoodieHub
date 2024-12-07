using FoodieHub.API.Data.Entities;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _service;

        public FavoritesController(IFavoriteService service)
        {
            _service = service;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Favorite>> Create([FromBody]Favorite favorite)
        {
            var result = await _service.Create(favorite);
            if (result == null) return BadRequest();
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute]int id)
        {
            bool result = await _service.Delete(id);
            return result? NoContent():BadRequest();
        }

       /* [HttpGet("ArticleRanking")]
        public async Task<IActionResult> GetArticleRanking()
        {
            var result = await _service.GetArticleRanking();
            return StatusCode(result.StatusCode, result);
        }*/
    }
}
