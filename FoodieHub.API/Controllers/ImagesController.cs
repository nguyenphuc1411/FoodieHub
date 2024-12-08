using FoodieHub.API.Models.DTOs.UploadImage;
using FoodieHub.API.Repositories.Interfaces;
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

        [HttpPost]
        public async Task<ActionResult> Create([FromForm] UploadImageDTO dto)
        {
            var result = await service.UploadImageByName(dto);

            return result ? Ok(): BadRequest();
        }

        [HttpDelete("{path}")]
        public async Task<ActionResult> Delete([FromRoute] string path)
        {
            try
            {
                await service.DeleteImage(path);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }       
        }

    }
}
