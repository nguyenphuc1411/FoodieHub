using FoodieHub.API.Models.DTOs.Statistics;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IStatisticsService
    {
        Task<StatisticsDTO<int>> GetOrder(string by);
        Task<StatisticsDTO<decimal>> GetRevenue(string by);
        Task<StatisticsDTO<int>> GetCustomer(string by);

        Task<List<TopSelling>> TopSelling(string by);
        Task<List<TopSelling>> TopRevenue(string by);

        Task<List<OrderReport>> OrderReports(string by);
        Task<List<RevenueReport>> RevenueReports(string by);
    }
}
