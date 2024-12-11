using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Comment;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public CommentService(AppDbContext context, IAuthService authService, IMapper mapper)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<Comment?> Create(CommentDTO entity)
        {
            string userID = _authService.GetUserID();
            var newComment = _mapper.Map<Comment>(entity);
            newComment.UserID = userID;
            await _context.Comments.AddAsync(newComment);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return newComment;
            return null;
        }
        public async Task<bool> Delete(int commentID)
        {
            var comment = await _context.Comments.FindAsync(commentID);
            if (comment == null) return false;
            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Edit(int id, CommentDTO entity)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;
            comment.CommentContent = entity.CommentContent;
            comment.CommentedAt = DateTime.Now;
            _context.Comments.Update(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<GetCommentDTO>> GetByArticle(int id)
        {
            return await _context.Comments.Where(x => x.ArticleID == id)
                .ProjectTo<GetCommentDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(x=>x.CommentedAt).ToListAsync();
        }

        public async Task<IEnumerable<GetCommentDTO>> GetByRecipe(int id)
        {
            return await _context.Comments.Where(x => x.RecipeID == id)
               .ProjectTo<GetCommentDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.CommentedAt).ToListAsync();
        }
    }
}
