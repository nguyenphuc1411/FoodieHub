namespace FoodieHub.API.Models.QueryModel
{
    public class QueryUserModel
    {
        public string? Email { get; set; } = default!;
        public string? Role { get; set; } = default!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
