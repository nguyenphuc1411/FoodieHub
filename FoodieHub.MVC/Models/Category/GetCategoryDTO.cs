namespace FoodieHub.MVC.Models.Category
{
    public class GetCategoryDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
