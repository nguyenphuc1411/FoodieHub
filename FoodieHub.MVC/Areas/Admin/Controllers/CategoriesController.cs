using FoodieHub.MVC.Models.DTOs;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Categories;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryProductService;
        private readonly IArticleCategoryService _articleCategoryService;
        private readonly IRecipeCategoryService _recipeCategoryService;
        public CategoriesController(ICategoryService serviceProductService, IArticleCategoryService articleCategoryService, IRecipeCategoryService recipeCategoryService)
        {
            _articleCategoryService = articleCategoryService;
            _categoryProductService = serviceProductService;
            _recipeCategoryService = recipeCategoryService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductCategory(CategoryDTO category)
        {
            if (ModelState.IsValid)
            {
                await _categoryProductService.AddNewProductCategory(category);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductCategory(string id)
        {
            await _categoryProductService.DeleteProductCategory(int.Parse(id));

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProductCategory(CategoryDTO category)
        {
            var obj = await _categoryProductService.UpdateProductCategory(category);
            return RedirectToAction("Index");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticleCategory(ArticleCategoryDTO articleCategoryDTO)
        {
            if (ModelState.IsValid)
            {
                var obj = await _articleCategoryService.AddArticleCategory(articleCategoryDTO);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetArticleCategoryOn(ArticleCategoryDTO articleCategoryDTO)
        {
            var newCate = new ArticleCategoryDTO
            {
                CategoryID = articleCategoryDTO.CategoryID,
                CategoryName = articleCategoryDTO.CategoryName,
                IsDeleted = false
            };
            var obj = await _articleCategoryService.UpdateArticleCategory(articleCategoryDTO);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetArticleCategoryOff(ArticleCategoryDTO articleCategoryDTO)
        {
            var newCate = new ArticleCategoryDTO
            {
                CategoryID = articleCategoryDTO.CategoryID,
                CategoryName = articleCategoryDTO.CategoryName,
                IsDeleted = true
            };
            var obj = await _articleCategoryService.UpdateArticleCategory(newCate);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetArticleCategoryName(ArticleCategoryDTO articleCategoryDTO)
        {
            var newCate = new ArticleCategoryDTO
            {
                CategoryID = articleCategoryDTO.CategoryID,
                CategoryName = articleCategoryDTO.CategoryName,
                IsDeleted = articleCategoryDTO.IsDeleted
            };
            var obj = await _articleCategoryService.UpdateArticleCategory(newCate);
            return RedirectToAction("Index");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateRecipeCategory(RecipeCategoryDTO recipeCategoryDTO)
        {
            if (ModelState.IsValid)
            {

                var result = await _recipeCategoryService.AddRecipeCategory(recipeCategoryDTO);

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRecipeCategoryOn(RecipeCategoryStatusDTO recipeCategoryDTO)
        {
            var obj = new RecipeCategoryStatusDTO
            {
                CategoryID = recipeCategoryDTO.CategoryID,
                IsActice = true,
                IsDeleted = false
            };
            var result = await _recipeCategoryService.UpdateRecipeStatus(obj);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRecipeCategoryOff(RecipeCategoryStatusDTO recipeCategoryDTO)
        {
            var obj = new RecipeCategoryStatusDTO
            {
                CategoryID = recipeCategoryDTO.CategoryID,
                IsActice = false,
                IsDeleted = true
            };
            var result = await _recipeCategoryService.UpdateRecipeStatus(obj);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateRecipeCategory(RecipeCategoryDTO recipeCategoryDTO)
        {
            if (recipeCategoryDTO.ImageURL == null)
            {
                var obj = new RecipeCategoryNoneImgDTO
                {
                    CategoryID = recipeCategoryDTO.CategoryID,
                    CategoryName = recipeCategoryDTO.CategoryName
                };

                var result = _recipeCategoryService.UpdateRecipeCategoryNoneImg(obj);
            }
            else if (recipeCategoryDTO.ImageURL != null)
            {
                var obj = new RecipeCategoryWithImgDTO
                {
                    CategoryID = recipeCategoryDTO.CategoryID,
                    CategoryName = recipeCategoryDTO.CategoryName,
                    ImageURL = recipeCategoryDTO.ImageURL
                };

                var result = await _recipeCategoryService.UpdateRecipeCategoryWithImg(obj);
            }

            return RedirectToAction("Index");
        }

    }
}