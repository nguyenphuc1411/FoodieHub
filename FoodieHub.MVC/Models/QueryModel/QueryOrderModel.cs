namespace FoodieHub.MVC.Models.QueryModel
{
    public class QueryOrderModel : QueryModel
    {
        public string? Status { get; set; }
        public DateOnly? OrderDate { get; set; }
    }
}
