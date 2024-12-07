using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FoodieHub.API.Services.Implementations
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

        public async Task<ServiceResponse> Create(CreateArticle article)
        {
            var uploadImgResult = await _imgHelper.UploadImage(article.File, "Articles");
            if (uploadImgResult.Success)
            {
                var userID = _authService.GetUserID();
                var newArticle = new Article
                {
                    Title = article.Title,
                    Description = article.Description,
                    UserID =userID,
                    CategoryID = article.CategoryID,
                    MainImage = uploadImgResult.FilePath.ToString()??""
                };
                await _context.Articles.AddAsync(newArticle);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "The article has been successfully created",
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Failed to create the article. Please try again later",
                        StatusCode = 400
                    };
                }
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Falied to upload the image. Please try again later",
                StatusCode = 400
            };
        }

        public async Task<ServiceResponse> Update(int articleID, UpdateArticle article)
        {
            var userID = _authService.GetUserID();
            var artilceExists = await _context.Articles.FindAsync(articleID); 
            if(artilceExists==null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"No article found with given ID: {articleID}",
                    StatusCode = 404
                };
            artilceExists.Title = article.Title;
            artilceExists.Description = article.Description;
            artilceExists.UserID = userID;
            artilceExists.CategoryID = article.CategoryID;
            artilceExists.UpdatedAt = DateTime.Now;
            if (article.File != null)
            {
                var uploadResult = await _imgHelper.UploadImage(article.File, "Articles");
                if (uploadResult.Success)
                {
                    _imgHelper.DeleteImage(artilceExists.MainImage);
                    artilceExists.MainImage = uploadResult.FilePath.ToString() ?? "";
                }
                else {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Failed to upload the image. Please try again later",
                        StatusCode = 400
                    };
                }
                
            }
            _context.Articles.Update(artilceExists);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "The artcile has been successfully updated",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to update the article. Please try again later",
                    StatusCode = 400
                };
            }
        }

        public async Task<ServiceResponse> Get(string? search, int? pageSize, int? currentPage)
        {
            var articles = _context.Articles
                .Where(x=>x.IsDeleted==false)
                .ProjectTo<GetArticles>(_mapper.ConfigurationProvider)
                         .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                articles = articles.Where(x => x.Title.ToLower().Contains(search.ToLower()));

            }
            if (pageSize.HasValue && currentPage.HasValue)
            {
                var list = await articles.ToListAsync();
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Get articles successfully",
                    Data = list.Paginate(pageSize.Value, currentPage.Value),
                    StatusCode = 200
                };
            }
            return new ServiceResponse
            {
                Success = true,
                Message = "Get articles successfully",
                Data = await articles.ToListAsync(),
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> GetDetail(int articleID)
        {
            var result = await _context.Articles
                .Where(x => x.ArticleID == articleID && x.IsDeleted == false)
                .ProjectTo<GetArticleDetail>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            if (result == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Not found this article",
                    StatusCode = 404
                };
            return new ServiceResponse
            {
                Success = true,
                Message = "Get article success",
                Data = result,
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> HardDelete(int articleID)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var article = await _context.Articles.FindAsync(articleID);
                if (article == null)
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Not found this article",
                        StatusCode = 404
                    };
                var listComments = await _context.Comments.Where(x=>x.ArticleID==articleID).ToListAsync();
                var listFavorites = await _context.Favorites.Where(x=>x.ArticleID== articleID).ToListAsync();
                if (listComments.Count() > 0)
                {
                    foreach (var comment in listComments)
                    {
                        _context.Comments.Remove(comment);
                    }
                }
                if (listFavorites.Count() > 0)
                {
                    foreach (var favorite in listFavorites)
                    {
                        _context.Favorites.Remove(favorite);
                    }
                }
                var imgPath = article.MainImage;
                _context.Articles.Remove(article);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _imgHelper.DeleteImage(imgPath);
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "Delete article successfully",
                        StatusCode = 200
                    };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Delete article failed",
                        StatusCode = 400
                    };
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Server error. Please try again later",
                    StatusCode = 500
                };
            }
           
        }

        public async Task<ServiceResponse> SoftDelete(int articleID)
        {
            var article = await _context.Articles.FindAsync(articleID);
            if(article == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"No  article found with given ID {articleID}",
                    StatusCode = 404
                };
            article.IsDeleted = true;
            _context.Articles.Update(article);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "The article has been successfully deleted",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Falied to delete the article. Please try again later",
                    StatusCode = 400
                };
            }
        }

        public async Task<ServiceResponse> Top10Article()
        {
            var toparticle = await _context.Articles.Select(x => new
            {
                x.ArticleID,
                x.Title,
                CountFavorite = x.FavoriteArticles.Count()
            }).OrderByDescending(x => x.CountFavorite).Take(10).ToListAsync();
            return new ServiceResponse
            {
                Success = true,
                Data = toparticle,
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> GetForAdmin(string? search, int? categoryID, bool? isDeleted, int pageSize, int currentPage)
        {
            var listarticles = _context.Articles.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                listarticles = listarticles.Where(x => x.Title.ToLower().Contains(search.ToLower()));
            }
            if(categoryID.HasValue)
            {
                listarticles = listarticles.Where(x => x.CategoryID == categoryID);
            }
            if(isDeleted.HasValue)
            {
                if (isDeleted.Value)
                {
                    listarticles = listarticles.Where(x=>x.IsDeleted);
                }            
            }
            else
            {
                listarticles = listarticles.Where(x => !x.IsDeleted);
            }
            var articles = await listarticles
            .ProjectTo<GetArticlesForAdmin>(_mapper.ConfigurationProvider).ToListAsync();
           
            return new ServiceResponse
            {
                Success = true,
                Message = "Get articles successfully",
                Data = articles.Paginate(pageSize,currentPage),
                StatusCode = 200
            };
        
        }

        public async Task<ServiceResponse> GetDetailForAdmin(int articleID)
        {
            var result = await _context.Articles
                .Where(x=>x.ArticleID == articleID)
                .Select(x=>new
                {
                    x.ArticleID,
                    x.Title,
                    x.Description,
                    x.MainImage,
                    x.CategoryID,
                    x.CreatedAt,
                    x.UpdatedAt,
                    Creator = x.User.Fullname
                })
                .FirstOrDefaultAsync();
            if (result == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Not found this article",
                    StatusCode = 404
                };
            return new ServiceResponse
            {
                Success = true,
                Message = "Get article success",
                Data = result,
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> Restore(int articleID)
        {
            var article = await _context.Articles.FindAsync(articleID);
            if (article == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"No  article found with given ID {articleID}",
                    StatusCode = 404
                };
            article.IsDeleted = false;
            _context.Articles.Update(article);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "The article has been successfully restored",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Falied to retore the article. Please try again later",
                    StatusCode = 400
                };
            }
        }
    }
}
