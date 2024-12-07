using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.User
{
    public class UpdateUser
    {
        [Required]
        [MinLength(4)]
        public string Fullname { get; set; }
        [MaxLength(255)]
        public string? Bio { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public IFormFile? File { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        public string? OldPassword { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        public string? NewPassword { get; set; }
    }
}
