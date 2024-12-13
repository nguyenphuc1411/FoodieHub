﻿using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Comment;
using FoodieHub.MVC.Models.QueryModel;

namespace FoodieHub.MVC.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IFavoriteService _favoriteService;
        private readonly ICommentService _commentService;
        public RecipesController(IRecipeService recipeService, IFavoriteService favoriteService, ICommentService commentService)
        {
            _recipeService = recipeService;
            _favoriteService = favoriteService;
            _commentService = commentService;
        }
        [ValidateTokenForUser]
        public IActionResult Create()
        {          
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeDTO recipe)
        {
            if (ModelState.IsValid)
            {
                if (!recipe.Ingredients.Any() || !recipe.RecipeSteps.Any())
                {
                    NotificationHelper.SetErrorNotification(this,"List ingredient and step is required");
                    return View();
                }
                bool result = await _recipeService.Create(recipe);
                if (result)
                {
                    NotificationHelper.SetSuccessNotification(this);
                    return Redirect("/account/recipes");
                }
                else NotificationHelper.SetErrorNotification(this);               
            }         
            return View(recipe);
        }




        public async Task<IActionResult> Edit(int id)
        {
            var response = await _recipeService.GetByID(id);
            if(response != null)
            {
                var edit = new UpdateRecipeDTO
                {
                    RecipeID = id,
                    Title = response.Title,
                    CookTime = response.CookTime,
                    Serves = response.Serves,
                    Description = response.Description,
                    IsActive = response.IsActive,
                    ImageURL = response.ImageURL,
                    CategoryID= response.CategoryID,
                    RecipeSteps = response.Steps,
                    Ingredients = response.Ingredients,
                };
                return View(edit);
            }
            NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Recipes", "Account");
        }

        [HttpPost]
        public IActionResult Edit(UpdateRecipeDTO update)
        {
            if (ModelState.IsValid)
            {
               
            }
            return View(update);
        }


        public async Task<IActionResult> Index(QueryRecipeModel query)
        {
            // Lấy danh sách công thức
            query.IsActive = true;
            var recipes = await _recipeService.GetAll(query);
            ViewBag.Query = query;
            return View(recipes);
        }



        // Action hiển thị chi tiết bài viết
        public async Task<IActionResult> Detail(int id)
        {
            var data = await _recipeService.GetByID(id);
            if (data == null || !data.IsActive)
            {
                NotificationHelper.SetErrorNotification(this,"Not found this recipe");
                return RedirectToAction("Index");
            }
            ViewBag.UserID = Request.GetCookie("UserID");
            return View(data);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite(int id)
        {
            var newFavorite = new FavoriteDTO { RecipeID = id };
            bool result = await _favoriteService.Create(newFavorite);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return Redirect("/Recipes/Detail/" + id);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(int id)
        {
            bool result = await _favoriteService.Delete(new FavoriteDTO
            {
                RecipeID = id
            });
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id });
        } 
        [ValidateTokenForUser]
        public async Task<IActionResult> Rating(int recipeID, int ratingValue)
        {
            var rating = new CreateRatingDTO
            {
                RecipeID = recipeID,
                RatingValue = ratingValue
            };
            bool result = await _recipeService.Rating(rating);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return Redirect("/Recipes/Detail/" + recipeID);
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recipeService.Delete(id);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Recipes", "Account");
        }

        public async Task<IActionResult> CreateComment(CommentDTO comment)
        {
            bool result = await _commentService.Create(comment);
            if(result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = comment.RecipeID});
        }

        public async Task<IActionResult> DeleteComment(int id,int recipeID)
        {
            bool result = await _commentService.Delete(id);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = recipeID });
        }

        public async Task<IActionResult> EditComment(int CommentID,string CommentContent,int RecipeID)
        {
            bool result = await _commentService.Edit(CommentID,new CommentDTO
            {
                CommentContent = CommentContent
            });
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = RecipeID });
        }
    }
}
