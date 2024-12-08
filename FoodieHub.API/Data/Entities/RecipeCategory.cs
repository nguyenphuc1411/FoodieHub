using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class RecipeCategory
    {
        [Key]
        public int CategoryID { get; set; }


        [Column(TypeName = "nvarchar(50)")]
        public string CategoryName { get; set; } = default!;


        [Column(TypeName = "varchar(255)")]
        public string ImageURL { get; set; } = default!;

        // Foreign Key Collections
        public ICollection<Recipe> Recipes { get; set; } = default!;
    }
}
