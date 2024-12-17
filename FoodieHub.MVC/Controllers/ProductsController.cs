using FoodieHub.MVC.Extentions;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace FoodieHub.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductImageService _productImageService;
        private readonly IProductService _productService;
        

        public ProductsController(IProductService productService, ICategoryService categoryService, IProductImageService productImageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;
        }

        public async Task<IActionResult> Index(string priceFilter = "", string categoryFilter = "", string sortBy = "Recommended", string searchName = "", int page = 1, int pageSize = 8)
        {
            var products = await _productService.GetAll();

            // Chỉ lấy những sản phẩm có IsActive == true
            products = products.Where(p => p.IsActive == true).ToList();

            // Lọc theo khoảng giá
            products = priceFilter switch
            {
                "Under25" => products.Where(p => p.Price < 25).ToList(),
                "25to50" => products.Where(p => p.Price >= 25 && p.Price <= 50).ToList(),
                "50to100" => products.Where(p => p.Price >= 50 && p.Price <= 100).ToList(),
                "100to200" => products.Where(p => p.Price >= 100 && p.Price <= 200).ToList(),
                _ => products
            };

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                products = products.Where(p => p.CategoryID.ToString() == categoryFilter).ToList();
            }

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(searchName))
            {
                products = products
                    .Where(p => RemoveDiacritics(p.ProductName)
                    .Contains(RemoveDiacritics(searchName), StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sắp xếp
            products = sortBy switch
            {
                "PriceLowToHigh" => products.OrderBy(p => p.Price).ToList(),
                "PriceHighToLow" => products.OrderByDescending(p => p.Price).ToList(),
                "NameAToZ" => products.OrderBy(p => p.ProductName).ToList(),
                "NameZToA" => products.OrderByDescending(p => p.ProductName).ToList(),
                _ => products // Recommended
            };

            // Phân trang
            var paginatedResult = products.Paginate(pageSize, page);

            ViewBag.CurrentPage = paginatedResult.Page;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.PageSize = paginatedResult.PageSize;

            return View(paginatedResult.Items); // Trả về chỉ danh sách sản phẩm của trang hiện tại
        }






        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
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

    }
}