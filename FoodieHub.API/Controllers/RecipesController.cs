using Azure;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Services.Interfaces;
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

        [Authorize]  
        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeDTO recipeDTO)
        {
            var result = await _service.Create(recipeDTO);
            return result ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpPut("{recipeID}")]
        public async Task<IActionResult> Update(int recipeID,UpdateRecipe recipeDTO)
        {
            var response = await _service.Update(recipeID,recipeDTO);
            return StatusCode(response.StatusCode, response);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("foradmin/{articleID}")]
        public async Task<IActionResult> GetDetailForAdmin(int articleID)
        {
            var result = await _service.GetDetailForAdmin(articleID);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("foradmin")]
        public async Task<IActionResult> GetForAdmin(string? search, int? categoryID,bool? isActive ,bool? isDeleted, int pageSize, int currentPage)
        {
            var result = await _service.GetForAdmin(search, categoryID,isActive ,isDeleted, pageSize, currentPage);
            return StatusCode(result.StatusCode, result);
        }   
        [Authorize]
        [HttpPost("rating")]
        public async Task<IActionResult> Rating(RatingDTO ratingDTO)
        {
            var result = await _service.Rating(ratingDTO);
            return StatusCode(result.StatusCode, result);
        }



        [HttpGet("{recipeID}")]
        public async Task<IActionResult> GetDetail(int recipeID)
        {
            var result = await _service.GetDetail(recipeID);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet]
        public async Task<IActionResult> Get(string? search,int? pageSize,int? currentPage)
        {
            var result = await _service.Get(search,pageSize,currentPage);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("user/{userid}")]
        public async Task<IActionResult> GetByUser(string userid)
        {
            var result = await _service.GetByUser(userid);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetOfUser()
        {
            var result = await _service.GetOfUser();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetRecipeOfUserForAdmin(string status)
        {
            var result = await _service.GetRecipeOfUserForAdmin(status);
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
      
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOfUser(int id)
        {
            var result = await _service.DeleteOfUser(id);
            return result ? NoContent() : BadRequest();
        }

        [Authorize]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> GetDetailForEdit(int id)
        {
            var result = await _service.GetDetailForEdit(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("{id}/user")]
        public async Task<IActionResult> DeleteRecipeOfUser(int id)
        {
            var result = await _service.DeleteRecipeOfUser(id);
            return result ? Ok():BadRequest();
        }
    }
}
