using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Comment;

namespace FoodieHub.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Comment?> Create(Comment entity);
        Task<bool> Edit(int id,Comment entity);
        Task<IEnumerable<CommentDTO>> GetByRecipe(int id);
        Task<IEnumerable<CommentDTO>> GetByArticle(int id);
        Task<bool> Delete(int commentID);
    }
}
