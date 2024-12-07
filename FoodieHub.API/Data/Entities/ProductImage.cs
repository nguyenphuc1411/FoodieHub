using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class ProductImage
    {
        [Key]
        public int ImageID { get; set; }


        [Column(TypeName = "varchar(255)")]
        public string ImageURL { get; set; } = default!;

        // Foregin Key Property
        public int ProductID { get; set; }

        // Foreign Key Link
        public Product Product { get; set; } = default!;
    }
}
