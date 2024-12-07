using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string PaymentMethod { get; set; }= default!;

        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public int OrderID { get; set; }
        public Order Order { get; set; } = default!;
    }
}
