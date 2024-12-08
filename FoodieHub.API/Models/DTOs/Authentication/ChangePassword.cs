using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Authentication
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Old Password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        public string OldPassword { get; set; } = default!;

        [Required(ErrorMessage = "New Password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        public string NewPassword { get; set; } = default!;
    }
}
