namespace FoodieHub.MVC.Models.QueryModel
{
    public class QueryModel
    {
        public string? SearchItem { get; set; }
        public string? SortBy { get; set; }
        public bool Ascending { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
