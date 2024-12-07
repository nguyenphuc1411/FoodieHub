using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }


        [Column(TypeName = "nvarchar(50)")]
        public string CategoryName { get; set; } = default!;

        // Foreign Key Collections
        public ICollection<Product> Products { get; set; } = default!;
    }
}
