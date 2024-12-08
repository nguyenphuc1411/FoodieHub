using FoodieHub.API.Models.DTOs.Review;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetListReview(id);
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(ReviewDTO reviewDTO)
        {
            var result = await _service.AddNewReview(reviewDTO);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put(ReviewDTO reviewDTO)
        {
            var result = await _service.UpdateReview(reviewDTO);
            return StatusCode(result.StatusCode,result);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteNewReview(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}
