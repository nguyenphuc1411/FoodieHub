using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly ImageExtentions _imgHelper;
        private readonly IMapper _mapper;
        public ArticleService(AppDbContext context, IAuthService authService, ImageExtentions imgHelper, IMapper mapper)
        {
            _context = context;
            _authService = authService;
            _imgHelper = imgHelper;
            _mapper = mapper;
        }

        public async Task<bool> Create(CreateArticleDTO article)
        {
            var uploadImgResult = await _imgHelper.UploadImage(article.File, "Articles");
            if (!uploadImgResult.Success) return false;

            var userID = _authService.GetUserID();
            var newArticle = new Article
            {
                Title = article.Title,
                Description = article.Description,
                IsActive = article.IsActive,
                UserID =userID,
                CategoryID = article.CategoryID,
                MainImage = uploadImgResult.FilePath.ToString()??""
            };
            await _context.Articles.AddAsync(newArticle);
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<bool> Update(UpdateArticleDTO article)
        {
            var userID = _authService.GetUserID();
            var artilceExists = await _context.Articles.FindAsync(article.ArticleID); 
            if(artilceExists==null) return false;
            _mapper.Map(article, artilceExists);
            artilceExists.UserID=userID;
            if (article.File != null)
            {
                var uploadResult = await _imgHelper.UploadImage(article.File, "Articles");
                if (uploadResult.Success)
                {
                    _imgHelper.DeleteImage(artilceExists.MainImage);
                    artilceExists.MainImage = uploadResult.FilePath.ToString() ?? "";
                }              
            }
            _context.Articles.Update(artilceExists);
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<PaginatedModel<GetArticleDTO>> Get(QueryArticleModel query)
        {
            var articles = _context.Articles
                .ProjectTo<GetArticleDTO>(_mapper.ConfigurationProvider)
                         .AsQueryable();
            if (query.CategoryID.HasValue)
            {
                articles = articles.Where(x => x.CategoryID== query.CategoryID.Value);

            }
            return await articles.ApplyQuery(query,x=>x.Title);
        }

        public async Task<GetArticleDTO?> GetByID(int id)
        {
            return await _context.Articles
                .Where(x => x.ArticleID == id)
                .ProjectTo<GetArticleDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }
        public async Task<bool> Delete(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null) return false;
                var listComments = await _context.Comments.Where(x=>x.ArticleID==id).ToListAsync();
                var listFavorites = await _context.Favorites.Where(x=>x.ArticleID== id).ToListAsync();
                
                foreach (var comment in listComments)
                {
                    _context.Comments.Remove(comment);
                }
               
                foreach (var favorite in listFavorites)
                {
                    _context.Favorites.Remove(favorite);
                }                
                var imgPath = article.MainImage;
                _context.Articles.Remove(article);
                int countEntity = listComments.Count() +listComments.Count();
                var result = await _context.SaveChangesAsync();
                if (result > countEntity)
                {
                    await transaction.CommitAsync();
                    _imgHelper.DeleteImage(imgPath);
                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                return false ;
            }
           
        }

        public async Task<IEnumerable<GetArticleDTO>> GetOfUser(string userID)
        {
            return await _context.Articles
                .Where(x => x.UserID == userID)
                .ProjectTo<GetArticleDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArticleByCategory>> GetByCategory()
        {
            var articlesByCategory = await _context.Articles
                .GroupBy(a => a.ArticleCategory.CategoryID)
                .Select(g => new ArticleByCategory
                {
                    CategoryID = g.Key,
                    CategoryName = g.FirstOrDefault().ArticleCategory.CategoryName, // Lấy CategoryName từ bài viết đầu tiên
                                                                                    // Lấy bài viết có nhiều lượt thích nhất
                    FeatureArticle = _mapper.Map<GetArticleDTO>(g.OrderByDescending(a => a.FavoriteArticles.Count()).FirstOrDefault() ?? new Article()),
                    // Lấy danh sách các bài viết mới nhất
                    LastedArticles = _mapper.Map<IEnumerable<GetArticleDTO>>(g.OrderByDescending(a => a.CreatedAt).ToList())
                }).ToListAsync(); // Sử dụng ToListAsync() để thực hiện async/await

            return articlesByCategory;
        }

    }
}
