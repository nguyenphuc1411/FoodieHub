using FoodieHub.API.Models.DTOs.Comment;
using FoodieHub.MVC.Models.Comment;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface ICommentService
    {
        Task<bool> Create(CreateCommentDTO comment);
        Task<IEnumerable<CommentDTO>> GetCommentRecipe(int id);
        Task<IEnumerable<CommentDTO>> GetCommentArticle(int id);
        Task<bool> Delete(int id);
    }
}