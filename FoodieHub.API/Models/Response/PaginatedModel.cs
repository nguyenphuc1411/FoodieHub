namespace FoodieHub.API.Models.Response
{
    public class PaginatedModel<T>
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = default!;
    }
}
