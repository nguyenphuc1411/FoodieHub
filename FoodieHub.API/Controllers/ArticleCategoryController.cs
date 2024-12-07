﻿using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleCategoryController : ControllerBase
    {
        private readonly IArticleCategoryService _ArticleCategoryService;

        public ArticleCategoryController(IArticleCategoryService ArticleCategoryService)
        {
            _ArticleCategoryService = ArticleCategoryService;
        }

        [HttpGet("getallarticlecategory")]
        public async Task<IActionResult> Get()
        {
            var result = await _ArticleCategoryService.GetAllArticleCategories();
            return Ok(result);
        }

        [HttpPost("addnewarticlecategory")]
        public async Task<IActionResult> Post(ArticleCategoryDTO category)
        {
            var result = await _ArticleCategoryService.AddArticleCategory(category);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("updatearticlecategory")]
        public async Task<IActionResult> Put(ArticleCategoryDTO category)
        {
            var result = await _ArticleCategoryService.UpdateArticleCategory(category);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("deletearticlecategory/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ArticleCategoryService.DeleteArticleCategory(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}