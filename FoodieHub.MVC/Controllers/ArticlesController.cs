using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Comment;
using FoodieHub.MVC.Models.QueryModel;

namespace FoodieHub.MVC.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly IFavoriteService _favoriteService;
        private readonly IArticleCategoryService categoryService;
        public ArticlesController(
            IArticleService articleService,
            ICommentService commentService,
            IHttpClientFactory httpClientFactory,
            IFavoriteService favoriteService,
            IArticleCategoryService categoryService)
        {
            _articleService = articleService;
            _commentService = commentService;
            _favoriteService = favoriteService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var queryFavorite = new QueryArticleModel
            {
                SortBy = "TotalFavorites",
            };
            var topArticles = await _articleService.Get(queryFavorite);

            var queryLasted = new QueryArticleModel
            {
                SortBy = "CreatedAt"
            };
            var lastedArticle = await _articleService.Get(queryLasted);

            var viewModel = new ArticleViewModel
            {
                TopArticles = topArticles.Items.Where(x=>x.IsActive),
                LatestArticlesList = lastedArticle.Items.Where(x=>x.IsActive),
            };
            var result = await _articleService.GetByCategory();
            ViewBag.Data = result;
            return View(viewModel);
        }


        public async Task<IActionResult> ViewAll(QueryArticleModel queryArticle)
        {
            var result = await _articleService.Get(queryArticle);
            if(result == null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index");
            }
            var categories = await categoryService.GetAll();
           ViewBag.Categories = categories;
           ViewBag.Query = queryArticle;
           return View(result);
        }  
        public async Task<IActionResult> Detail(int id, string order = "desc")
        {
            var data = await _articleService.GetByID(id);
            if (data == null || !data.IsActive)
            {
                NotificationHelper.SetErrorNotification(this, "Not found this article");
            }
            ViewBag.UserID = Request.GetCookie("UserID");
            return View(data);
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentDTO comment, string order = "desc")
        {
            if (ModelState.IsValid)
            {
                bool result = await _commentService.Create(comment);
                if(result)
                    NotificationHelper.SetSuccessNotification(this);
                else
                    NotificationHelper.SetErrorNotification(this);
            }
            return RedirectToAction("Detail", new { id = comment.ArticleID, order });
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> EditComment(int CommentID, string CommentContent, int articleID)
        {
            bool result = await _commentService.Edit(CommentID, new CommentDTO
            {
                CommentContent = CommentContent
            });
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = articleID });
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Delete(int commentID,int articleID ,string order = "desc")
        {
            bool result = await _commentService.Delete(commentID);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = articleID, order });
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite(int id)
        {
            var favorote = new FavoriteDTO { ArticleID = id };

            bool result = await _favoriteService.Create(favorote);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);

            return RedirectToAction("Detail", new { id });
        }
       

        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(int id)
        {
            var result = await _favoriteService.Delete(new FavoriteDTO
            {
                ArticleID = id
            });
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id });
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> DeleteComment(int id, int articleID)
        {
            bool result = await _commentService.Delete(id);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = articleID });
        }
    }
}
