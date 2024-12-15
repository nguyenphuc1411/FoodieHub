namespace FoodieHub.MVC.Models.Categories
{
    public class RecipeCategoryWithImgDTO
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }


        public IFormFile ImageURL { get; set; }

    }
}
