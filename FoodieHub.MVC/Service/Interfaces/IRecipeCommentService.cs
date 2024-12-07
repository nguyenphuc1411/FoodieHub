using FoodieHub.MVC.Models.CommentRecipe;
using FoodieHub.MVC.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IRecipeCommentService
    {
        Task<APIResponse> CreateRecipeCommentAsync(CreateRecipeComment comment);
        Task<APIResponse<List<GetComment>>> GetCommentsOfRecipeAsync(int recipeID, string order = "asc");
        Task<APIResponse> DeleteCommentAsync(int commentID, string type);
    }
}
