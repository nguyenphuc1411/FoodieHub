using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Areas.Admin.Models
{
    public class DiscountValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Lấy giá trị của DiscountType từ đối tượng đang được xác thực
            var discountTypeProperty = validationContext.ObjectType.GetProperty("DiscountType");
            if (discountTypeProperty == null)
            {
                return new ValidationResult("DiscountType is required.");
            }

            var discountType = discountTypeProperty.GetValue(validationContext.ObjectInstance) as string;
            if (string.IsNullOrEmpty(discountType))
            {
                return new ValidationResult("DiscountType cannot be null or empty.");
            }

            var discountValue = (decimal)value;

            // Kiểm tra loại DiscountType
            if (discountType == "Percentage")
            {
                if (discountValue < 0 || discountValue > 100)
                {
                    return new ValidationResult("Percentage DiscountValue must be between 0 and 100.");
                }
            }
            else if (discountType == "Fixed")
            {
                if (discountValue < 0)
                {
                    return new ValidationResult("FixedAmount DiscountValue must be a positive number.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
