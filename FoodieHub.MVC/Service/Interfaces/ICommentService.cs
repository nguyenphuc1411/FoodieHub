using FoodieHub.API.Models.DTOs.Comment;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface ICommentService
    {
        Task<bool> Create(CommentDTO comment);
        Task<IEnumerable<GetCommentDTO>> GetCommentRecipe(int id);
        Task<IEnumerable<GetCommentDTO>> GetCommentArticle(int id);
        Task<bool> Delete(int id);
    }
}