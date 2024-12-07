using FoodieHub.MVC.Models.Comment;
using FoodieHub.MVC.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface ICommentService
    {
        Task<APIResponse> CreateArticleCommentAsync(CreateArticleComment comment);
        Task<APIResponse<List<GetComment>>> GetCommentsOfArticleAsync(int articleID, string order = "asc");
        Task<APIResponse> DeleteCommentAsync(int commentID, string type);
    }
}