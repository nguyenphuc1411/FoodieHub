using FoodieHub.MVC.Models.DTOs;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Categories;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Helpers;

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
                var response = await _categoryProductService.AddNewProductCategory(category);

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
        public async Task<IActionResult> DeleteProductCategory(string id)
        {
            var response = await _categoryProductService.DeleteProductCategory(int.Parse(id));
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
        public async Task<IActionResult> EditProductCategory(CategoryDTO category)
        {
            var response = await _categoryProductService.UpdateProductCategory(category);
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
        public async Task<IActionResult> CreateArticleCategory(ArticleCategoryDTO articleCategoryDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _articleCategoryService.AddArticleCategory(articleCategoryDTO);
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
        public async Task<IActionResult> SetArticleCategoryOn(ArticleCategoryDTO articleCategoryDTO)
        {
            var newCate = new ArticleCategoryDTO
            {
                CategoryID = articleCategoryDTO.CategoryID,
                CategoryName = articleCategoryDTO.CategoryName
            };
            var response = await _articleCategoryService.UpdateArticleCategory(articleCategoryDTO);
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
        public async Task<IActionResult> SetArticleCategoryOff(ArticleCategoryDTO articleCategoryDTO)
        {
            var newCate = new ArticleCategoryDTO
            {
                CategoryID = articleCategoryDTO.CategoryID,
                CategoryName = articleCategoryDTO.CategoryName
            };
            var response = await _articleCategoryService.UpdateArticleCategory(newCate);
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
        public async Task<IActionResult> SetArticleCategoryName(ArticleCategoryDTO articleCategoryDTO)
        {
            var newCate = new ArticleCategoryDTO
            {
                CategoryID = articleCategoryDTO.CategoryID,
                CategoryName = articleCategoryDTO.CategoryName
            };
            var response = await _articleCategoryService.UpdateArticleCategory(newCate);
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

        public async Task<IActionResult> CreateRecipeCategory(RecipeCategoryDTO recipeCategoryDTO)
        {
            if (ModelState.IsValid)
            {

                var response = await _recipeCategoryService.AddRecipeCategory(recipeCategoryDTO);
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
        public async Task<IActionResult> SetRecipeCategoryOn(RecipeCategoryStatusDTO recipeCategoryDTO)
        {
            var obj = new RecipeCategoryStatusDTO
            {
                CategoryID = recipeCategoryDTO.CategoryID,
            };
            var response = await _recipeCategoryService.UpdateRecipeStatus(obj);
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
        public async Task<IActionResult> SetRecipeCategoryOff(RecipeCategoryStatusDTO recipeCategoryDTO)
        {
            var obj = new RecipeCategoryStatusDTO
            {
                CategoryID = recipeCategoryDTO.CategoryID,
            };
            var response = await _recipeCategoryService.UpdateRecipeStatus(obj);
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

        public async Task<IActionResult> UpdateRecipeCategory(RecipeCategoryDTO recipeCategoryDTO)
        {
            if (recipeCategoryDTO.ImageURL == null)
            {
                var obj = new RecipeCategoryNoneImgDTO
                {
                    CategoryID = recipeCategoryDTO.CategoryID,
                    CategoryName = recipeCategoryDTO.CategoryName
                };

                var response = await _recipeCategoryService.UpdateRecipeCategoryNoneImg(obj);
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
            else if (recipeCategoryDTO.ImageURL != null)
            {
                var obj = new RecipeCategoryWithImgDTO
                {
                    CategoryID = recipeCategoryDTO.CategoryID,
                    CategoryName = recipeCategoryDTO.CategoryName,
                    ImageURL = recipeCategoryDTO.ImageURL
                };

                var response = await _recipeCategoryService.UpdateRecipeCategoryWithImg(obj);
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

    }
}