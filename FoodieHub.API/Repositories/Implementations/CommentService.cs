using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Comment;
using FoodieHub.API.Models.Response;
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

        public async Task<Comment?> Create(Comment entity)
        {
            string userID = _authService.GetUserID();

            entity.UserID = userID;
            if (entity.UserID == null) return null;

            await _context.Comments.AddAsync(entity);
            var result = await _context.SaveChangesAsync();
            if (result > 0) return entity;
            return null;
        }
        public async Task<bool> Delete(int commentID)
        {
            var comment = await _context.Comments.FindAsync(commentID);
            if (comment == null) return false;
            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Edit(int id, Comment entity)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;
            string userId = _authService.GetUserID();
            if(comment.UserID!=userId) return false;
            comment = entity;
            _context.Comments.Update(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<object> Get(int id,string? order)
        {
            var comments = await _context.Comments.Where(x => x.RecipeID == id || x.ArticleID==id).Select(x => new
            {
                x.CommentID,
                x.CommentContent,
                x.CommentedAt,
                x.UserID,
                x.User.Avatar,
                x.User.Fullname,
            }).ToListAsync();
            return comments;
        }

        public async Task<IEnumerable<CommentDTO>> GetByArticle(int id)
        {
            return await _context.Comments.Where(x => x.ArticleID == id)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<IEnumerable<CommentDTO>> GetByRecipe(int id)
        {
            return await _context.Comments.Where(x => x.RecipeID == id)
               .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
