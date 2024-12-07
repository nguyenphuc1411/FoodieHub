using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class OrderDetail
    {

        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public int Discount { get; set; }

        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal TotalPrice { get; set; }

        // Foregin Key Property
        public int ProductID { get; set; }

        public int OrderID { get; set; }
        // Foreign Key Link
        public Product Product { get; set; } = default!;

        public Order Order { get; set; } = default!;
    }
}
