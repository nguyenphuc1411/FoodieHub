using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs
{
    public class PaymentDTO
    {
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public int OrderID { get; set; }
    }
}
