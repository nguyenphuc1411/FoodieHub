using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Product;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductImageService _productImageService;
        public ProductsController(IProductService productService, ICategoryService categoryService, IProductImageService productImageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(string searchName = "", int? categoryId = null)
        {
            // Lấy danh sách sản phẩm
            var products = await _productService.GetAll();

            if (!string.IsNullOrEmpty(searchName))
            {
                products = products
                   .Where(p => RemoveDiacritics(p.ProductName)
                   .Contains(RemoveDiacritics(searchName), StringComparison.OrdinalIgnoreCase))
                   .ToList();
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value).ToList();
            }

            // Lấy danh sách danh mục
            var categories = await _categoryService.GetAll();
            ViewBag.CategoryID = categories != null && categories.Any()
                ? new SelectList(categories, "CategoryID", "CategoryName")
                : new SelectList(Enumerable.Empty<SelectListItem>());

            return View(products);
        }
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        //public async Task<IActionResult> CreateNewProduct()
        //{
        //    var categories = await _categoryService.GetAll();
        //    if (categories == null || !categories.Any())
        //    {
        //        throw new Exception("Không thể lấy danh mục hoặc danh sách danh mục rỗng.");
        //    }

        //    ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

        //    return View();
        //}



        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProduct(ProductDTO productDTO)
        {
            var response = await _productService.AddProduct(productDTO);

            if (!response.Success)
            {
                NotificationHelper.SetErrorNotification(this,response.Message);
                
                return RedirectToAction("Index");
            }
            else
            {
                if (productDTO.Images != null)
                {
                    foreach (var item in productDTO.Images)
                    {
                        var img= new ProductImageDTO
                        {
                            ImageURL = item,
                            ProductID = int.Parse(response.Data.ToString()),
                        };
                        var response1 = await _productImageService.AddImageProduct(img);
                    }
                }

                NotificationHelper.SetSuccessNotification(this, response.Message);
                return RedirectToAction("Index");
            }
        }




        //// GET: Admin/Products/Edit/5
        //public async Task<IActionResult> EditProduct(int id)
        //{
        //    var product = await _productService.GetById(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product.Data);
        //}

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(GetProductDTO productDTO)
        {
            if (productDTO.ProductID == null)
            {
                return BadRequest();
            }
            if(productDTO.ImgFileUpdate == null)
            {
                var obj = new ProductNoneImgDTO
                {
                    ProductID = productDTO.ProductID,
                    ProductName = productDTO.ProductName,
                    Price = productDTO.Price,
                    ShelfLife = productDTO.ShelfLife,
                    CategoryID = productDTO.CategoryID,
                    StockQuantity = productDTO.StockQuantity,
                    Discount = productDTO.Discount,
                    Description = productDTO.Description,

                };
                var response = await _productService.UpdateProductNoneImg(obj);
                if (!response.Success)
                {
                    NotificationHelper.SetErrorNotification(this, response.Message);
                    return RedirectToAction("Index");
                }
                else
                {
                    NotificationHelper.SetSuccessNotification(this, response.Message);
                    return RedirectToAction("Index");
                }
            }
            else if(productDTO.ImgFileUpdate != null)
            {
                var obj = new ProductWithImgDTO
                {
                    ProductID = productDTO.ProductID,
                    ProductName = productDTO.ProductName,
                    Price = productDTO.Price,
                    ShelfLife = productDTO.ShelfLife,
                    CategoryID = productDTO.CategoryID,
                    MainImage = productDTO.ImgFileUpdate,
                    StockQuantity = productDTO.StockQuantity,
                    Discount = productDTO.Discount,
                    Description = productDTO.Description,

                };
                var response = await _productService.UpdateProductWithImg(obj);
                if (!response.Success)
                {
                    NotificationHelper.SetErrorNotification(this, response.Message);
                    return RedirectToAction("Index");
                }
                else
                {
                    NotificationHelper.SetSuccessNotification(this, response.Message);
                    return RedirectToAction("Index");
                }
            }
            
           
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleteImgResponse = await _productImageService.DeleteImgByProductID(id);
            if (!deleteImgResponse.Success)
            {
                // Xử lý lỗi nếu không xóa được hình ảnh
                ModelState.AddModelError(string.Empty, deleteImgResponse.Message);
                return RedirectToAction("Index");
            }

            // Xóa sản phẩm
            var response = await _productService.DeleteProduct(id);
            if (!response.Success)
            {
                NotificationHelper.SetErrorNotification(this, response.Message);
                return RedirectToAction("Index");
            }
            else
            {
                NotificationHelper.SetSuccessNotification(this, response.Message);
                return RedirectToAction("Index");
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetProductStatusOn(GetProductDTO getProductDTO)
        {
            var productDTO = new ProductStatusDTO
            {
                ProductID = getProductDTO.ProductID,
                IsActive = true // Đặt IsActive là true
            };

            var response = await _productService.SetProductStatusOn(productDTO);

            if (!response.Success)
            {
                NotificationHelper.SetErrorNotification(this, response.Message);
                return RedirectToAction("Index");
            }
            else
            {
                NotificationHelper.SetSuccessNotification(this, response.Message);
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetProductStatusOff(GetProductDTO getProductDTO)
        {
            var productDTO = new ProductStatusDTO
            { 
                ProductID = getProductDTO.ProductID,
                IsActive = false // Đặt IsActive là true
            };

            var response = await _productService.SetProductStatusOff(productDTO);

            if (!response.Success)
            {
                NotificationHelper.SetErrorNotification(this, response.Message);
                return RedirectToAction("Index");
            }
            else
            {
                NotificationHelper.SetSuccessNotification(this, response.Message);
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImageInImg(int ImageID)
        {
            var obj = await _productImageService.DeleteImage(ImageID);
            return RedirectToAction("Index");
        }


    }
}
