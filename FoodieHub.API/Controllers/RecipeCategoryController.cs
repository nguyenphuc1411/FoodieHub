using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeCategoryController : ControllerBase
    {
        private readonly IRecipeCategoryService _service;

        public RecipeCategoryController(IRecipeCategoryService recipeCategoryService)
        {
            _service = recipeCategoryService;
        }

        [HttpGet("getallrecipecategory")]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAllRecipeCategories();
            return Ok(result);
        }

        [HttpPost("addnewrecipecategory")]
        public async Task<IActionResult> Post(RecipeCategoryDTO category)
        {
            var result = await _service.AddRecipeCategory(category);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("updaterecipecategorytwithimg")]
        public async Task<IActionResult> PutWithImg(RecipeCategoryWithImgDTO category)
        {
            var result = await _service.UpdateRecipeCategoryWithImg(category);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("updaterecipecategorynoneimg")]
        public async Task<IActionResult> PutNoneImg(RecipeCategoryNoneImgDTO category)
        {
            var result = await _service.UpdateRecipeCategoryNoneImg(category);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("updatestatusrecipecategory")]
        public async Task<IActionResult> PutStatus(RecipeCategoryStatusDTO category)
        {
            var result = await _service.UpdateStatusCategory(category);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("deleterecipecategory/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteRecipeCategory(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
