using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Product;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodieHub.MVC.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> AddNewReview(ReviewDTO review)
        {
            var obj = await _service.AddNewReview(review);

            if (obj.Success)
            {
                // Chuyển hướng đến trang chi tiết sản phẩm và cuộn xuống phần listreview
                TempData["SuccessMessage"] = "Add review successfully!";

                // Giả sử review chứa ProductID, sử dụng ProductID của review để chuyển hướng đến trang chi tiết sản phẩm
                return RedirectToAction("Detail", "Products", new { id = review.ProductID });
            }
            else
            {
                // Nếu lỗi, có thể trả về trang hiện tại hoặc hiển thị thông báo lỗi
                return BadRequest(new { message = obj.Message });
            }
        }

        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> UpdateReview(UpdateReviewDTO review)
        {
            var obj = await _service.UpdateNewReview(review);

            if (obj.Success)
            {
                // Chuyển hướng đến trang chi tiết sản phẩm và cuộn xuống phần listreview
                TempData["SuccessMessage"] = "Update review successfully!";

                // Giả sử review chứa ProductID, sử dụng ProductID của review để chuyển hướng đến trang chi tiết sản phẩm
                return RedirectToAction("Detail", "Products", new { id = review.ProductID });
            }
            else
            {
                // Nếu lỗi, có thể trả về trang hiện tại hoặc hiển thị thông báo lỗi
                return BadRequest(new { message = obj.Message });
            }
        }

    }
}
