using FoodieHub.API.Models.DTOs.Product;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IImgService _service;

        public ProductImageController(IImgService productImageService)
        {
            _service = productImageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllImg();
            return Ok(response);
        }

        // Thêm hình ảnh sản phẩm
        [HttpPost]
        public async Task<IActionResult> AddImage([FromForm] ProductImageDTO img)
        {
            var response = await _service.AddImage(img);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        // Thêm nhiều hình ảnh sản phẩm
        [HttpPost("addmultipleimages")]
        public async Task<IActionResult> AddMultipleImages([FromForm] List<ProductImageDTO> imgs)
        {
            var response = await _service.AddMultipleImages(imgs);
            return StatusCode(response.StatusCode, response);
        }

        // Xóa hình ảnh sản phẩm
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var response = await _service.DeleteImage(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("deleteimgbyproductid/{id}")]
        public async Task<IActionResult> DeleteByProductID(int id)
        {
            var response = await _service.DeleteImgByProductID(id);
            return StatusCode(response.StatusCode, response);
        }



    }
}
