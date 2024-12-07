namespace FoodieHub.MVC.Areas.Admin.Models
{
    public class Statistics<T>
    {
        public T Total { get; set; }
        public int Trend { get; set; }
        public string GetBy { get; set; }
    }
}
