namespace FoodieHub.API.Data.Entities
{
    public class RecipeProduct
    {
        public int RecipeID { get; set; }
        public Recipe Recipe { get; set; } = default!;

        public int ProductID { get; set; }
        public Product Product { get; set; } = default!;
    }
}
