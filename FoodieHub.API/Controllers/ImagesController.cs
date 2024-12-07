using FoodieHub.API.Models.DTOs.UploadImage;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService service;

        public ImagesController(IImageService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("This is a GET request");
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromForm] UploadImageDTO dto)
        {
            var result = await service.UploadImageByName(dto);

            return result ? Ok(): BadRequest();
        }
    }
}
