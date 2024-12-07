using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Request
{
    public class ResetPassword
    {
        public string Email { get; set; }

        public string Token { get; set; }
        [Required]

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        public string NewPassword { get; set; }
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        [Compare("NewPassword", ErrorMessage = "Password not match")]
        public string ConfirmPassword { get; set; }
    }
}
