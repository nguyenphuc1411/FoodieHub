using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class ArticlesController : Controller
    {
        private readonly IArticleService service;

        public ArticlesController(IArticleService service)
        {
            this.service = service;
        }
     
        public async Task<IActionResult> Index(QueryArticleModel query)
        {
            var result = await service.Get(query);
            if(result == null)
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
        public async Task<IActionResult> Create(CreateArticleDTO article)
        {
            if (ModelState.IsValid)
            {
                bool result = await service.Create(article);
                if (result)
                {
                    NotificationHelper.SetSuccessNotification(this);
                    return RedirectToAction("Index");
                }else NotificationHelper.SetErrorNotification(this);
            }                        
            return View(article);
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
        public async Task<IActionResult> Edit(int id)
        {
            var result = await service.GetByID(id);
            if (result == null) { NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index");
            };              
            var article = new UpdateArticleDTO
            {
                ArticleID = result.ArticleID,
                Title = result.Title,
                Description = result.Description,
                CategoryID = result.CategoryID,
                IsActive = result.IsActive
            };
            Response.SetCookie("ImageCurrent",result.MainImage);
            ViewBag.CurrentImage=result.MainImage;
            return View(article);         
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateArticleDTO update)
        {
            if (ModelState.IsValid)
            {
                var result = await service.Update(update.ArticleID, update);
                if (result)
                {
                    NotificationHelper.SetSuccessNotification(this);
                    return RedirectToAction("Index");
                }
                else NotificationHelper.SetErrorNotification(this);
            }
            ViewBag.CurrentImage= Request.GetCookie("ImageCurrent");
            return View(update);
        }
          

        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.Delete(id);
            if (result)
            {
                NotificationHelper.SetSuccessNotification(this);
                return RedirectToAction("Index");
            }
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Index");
        }              
    }
}
