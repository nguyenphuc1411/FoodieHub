namespace FoodieHub.API.Models.DTOs.Statistics
{
    public class StatisticsDTO<T>
    {
        public T Total { get; set; }
        public int Trend { get; set; }
        public string GetBy { get; set; }
    }
}
