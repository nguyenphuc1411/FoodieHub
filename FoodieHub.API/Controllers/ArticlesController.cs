using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("foradmin")]
        public async Task<IActionResult> GetForAdmin(string? search, int? categoryID, bool? isDeleted, int pageSize, int currentPage)
        {
            var result = await _service.GetForAdmin(search, categoryID, isDeleted, pageSize, currentPage);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("foradmin/{articleID}")]
        public async Task<IActionResult> GetDetailForAdmin(int articleID)
        {
            var result = await _service.GetDetailForAdmin(articleID);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateArticle articleDTO)
        {
            var result = await _service.Create(articleDTO);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("{articleID}")]
        public async Task<IActionResult> Update(int articleID,UpdateArticle articleDTO)
        {
            var result = await _service.Update(articleID, articleDTO);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("soft/{articleID}")]
        public async Task<IActionResult> SoftDelete(int articleID)
        {
            var result = await _service.SoftDelete(articleID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.GetDetail(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? search, int? pageSize, int? currentPage)
        {
            var result = await _service.Get(search,pageSize,currentPage);
            return StatusCode(result.StatusCode, result);
        }
    }
}
