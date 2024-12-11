using FoodieHub.MVC.Models.Comment;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface ICommentService
    {
        Task<bool> Create(CommentDTO comment);
        Task<bool> Edit(int id, CommentDTO comment);
        Task<IEnumerable<GetCommentDTO>> GetCommentRecipe(int id);
        Task<IEnumerable<GetCommentDTO>> GetCommentArticle(int id);
        Task<bool> Delete(int id);
    }
}