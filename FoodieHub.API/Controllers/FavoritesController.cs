using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Favorite;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Repositories.Interfaces;
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
        [HttpPost]
        public async Task<ActionResult<Favorite>> Create([FromBody] CreateFavoriteDTO favorite)
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
        [Authorize]
        [HttpGet("recipes")]
        public async Task<ActionResult<IEnumerable<GetRecipeDTO>>> GetFR()
        {
            var result = await _service.GetFavoriteRecipe();
            return Ok(result);
        }
        [Authorize]
        [HttpGet("articles")]
        public async Task<ActionResult<IEnumerable<GetArticleDTO>>> GetFA()
        {
            var result = await _service.GetFavoriteArticle();
            return Ok(result);
        }
    }
}
