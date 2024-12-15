using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _service;

        public RecipesController(IRecipeService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedModel<GetRecipeDTO>>> Get([FromQuery] QueryRecipeModel query)
        {
            return Ok(await _service.Get(query));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateRecipeDTO recipeDTO)
        {
            if (!recipeDTO.Ingredients.Any() || !recipeDTO.RecipeSteps.Any())
            {
                return BadRequest();
            }
            var result = await _service.Create(recipeDTO);
            return result ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpPut("{recipeID}")]
        public async Task<IActionResult> Update(int recipeID,[FromForm] UpdateRecipeDTO recipeDTO)
        {
            if (recipeID != recipeDTO.RecipeID) return BadRequest();
            var result = await _service.Update(recipeDTO);
            return result ? NoContent() : BadRequest();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DetailRecipeDTO>> GetByID([FromRoute]int id)
        {
            var result = await _service.GetByID(id);
            if(result==null) return NotFound();
            return Ok(result);
        }

        [Authorize]
        [HttpPost("ratings")]
        public async Task<IActionResult> RatingRecipe(CreateRatingDTO ratingDTO)
        {
            var result = await _service.Rating(ratingDTO);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("users/{userid}")]
        public async Task<ActionResult<IEnumerable<GetRecipeDTO>>> GetByUser(string userid)
        {
            return Ok(await _service.GetByUser(userid));
        }
        [Authorize]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<GetRecipeDTO>>> GetOfUser()
        {
            return Ok(await _service.GetOfUser());
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
