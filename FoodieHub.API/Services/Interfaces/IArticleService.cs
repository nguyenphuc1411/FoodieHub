using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IArticleService
    {
        Task<ServiceResponse> Create(CreateArticle article);
        Task<ServiceResponse> Update(int articleID,UpdateArticle article);
        Task<ServiceResponse> Get(string? search,int? pageSize,int? currentPage);
        Task<ServiceResponse> GetForAdmin(string? search,int? categoryID,bool? isDeleted, int pageSize, int currentPage);
        Task<ServiceResponse> GetDetail(int articleID);
        Task<ServiceResponse> GetDetailForAdmin(int articleID);
        Task<ServiceResponse> SoftDelete(int articleID);
        Task<ServiceResponse> HardDelete(int articleID);
        Task<ServiceResponse> Restore(int articleID);
        Task<ServiceResponse> Top10Article();
    }
}
