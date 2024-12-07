using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string Data { get; set; } = default!;

        [Column(TypeName = "nvarchar(50)")]
        public string TokenType { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddMinutes(5);

        public bool IsUsed { get; set; } = false;

        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; } = default!;

        public ApplicationUser User { get; set; } = default!;
    }
}
