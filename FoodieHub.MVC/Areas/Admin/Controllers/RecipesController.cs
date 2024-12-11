using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Models.QueryModel;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class RecipesController : Controller
    {
        private readonly IRecipeService service;
        public RecipesController(IHttpClientFactory httpClientFactory, IRecipeService service)
        {
            this.service = service;
        }
        public async Task<IActionResult> Index(QueryRecipeModel query)
        {
            var result = await service.GetAll(query);
            if (result == null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index","Home");
            }
            ViewBag.Query = query;
            return View(result);     
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeDTO recipe)
        {
            if (ModelState.IsValid)
            {
                var result = await service.Create(recipe);
                if (result)
                {
                    NotificationHelper.SetSuccessNotification(this);
                    return RedirectToAction("Index");
                }
                NotificationHelper.SetErrorNotification(this);
            }           
            return View(recipe);

        }
        public async Task<IActionResult> Edit(int id)
        {
           
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CreateRecipeDTO edit)
        {
            if (ModelState.IsValid)
            {
                
            }  
            return View(edit);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var result = await service.GetByID(id);
            if (result == null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index");             
            }
            return View(result);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.GetByID(id);
            if (result == null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index");
            }
            return View(result);
        }
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var result = await service.Delete(id);
            if (result)
            {
                NotificationHelper.SetSuccessNotification(this);
            }
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Index");
        }            
    }
}