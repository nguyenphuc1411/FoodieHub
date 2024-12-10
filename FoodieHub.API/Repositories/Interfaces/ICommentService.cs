using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Comment;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface ICommentService
    {
        Task<Comment?> Create(CommentDTO entity);
        Task<bool> Edit(int id, CommentDTO entity);
        Task<IEnumerable<GetCommentDTO>> GetByRecipe(int id);
        Task<IEnumerable<GetCommentDTO>> GetByArticle(int id);
        Task<bool> Delete(int commentID);
    }
}
