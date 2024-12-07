﻿using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    // API quản lý tất cả danh mục bao gồm Danh Mục Sản Phẩm, Danh Mục Bài viết, Danh Mục Công thức
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet("getallcategories")]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAllCategories();
            return Ok(result);
        }

        [HttpGet("getcategorybyid/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var obj = await _service.GetCategoryById(id);
            return Ok(obj);
        }

        [HttpPost("addcategory")]

        public async Task<IActionResult> Post(CategoryDTO category)
        {
            var result = await _service.AddCategory(category);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("updatecategory")]
        public async Task<IActionResult> Put(CategoryDTO category)
        {
            var result = await _service.UpdateCategory(category);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("deletecategory/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteCategory(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}