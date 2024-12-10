namespace FoodieHub.MVC.Models.Categories
{
    public class GetRecipeCategoryDTO
    {
        public int CategoryID { get; set; }


        public string CategoryName { get; set; } = default!;


        public string ImageURL { get; set; } = default!;
    }
}
