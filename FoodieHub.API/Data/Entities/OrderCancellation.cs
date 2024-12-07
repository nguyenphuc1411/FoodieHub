using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class OrderCancellation
    {
        public int Id { get; set; }

        public DateTime CancelledDate { get; set; } = DateTime.Now;

        [Column(TypeName = "nvarchar(255)")]
        public string Reason { get; set; } = default!;

        public int OrderID { get; set; }
        public Order Order { get; set; } = default!;


        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

    }
}
