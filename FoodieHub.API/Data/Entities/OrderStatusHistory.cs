using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }

        public int OrderID { get; set; }
        public Order Order { get; set; } = default!;

        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; } = default!;

        public DateTime ChangeDate { get; set; } = DateTime.Now;
    }
}
