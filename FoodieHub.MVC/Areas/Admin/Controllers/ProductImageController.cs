using FoodieHub.MVC.Models;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductImageController : Controller
    {
        private readonly IProductImageService _productImageService;

        public ProductImageController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteImageInImg(int ImageID)
        //{
        //    var obj = await _productImageService.DeleteImage(ImageID);
        //    return Ok(obj);
        //}

        [HttpPost]
        public async Task<IActionResult> AddImg(ProductImageDTO productImage)
        {
            var result = await _productImageService.AddImageProduct(productImage);
            return RedirectToAction("Index", "Products");
        }
    }
}
