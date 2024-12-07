﻿
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models
{
    public class CategoryDTO
    {
        [Key]
        public int CategoryID { get; set; }


        [Required(ErrorMessage = "Category Name is required")] 
        [StringLength(50, ErrorMessage = "Category Name cannot exceed 50 characters")] 
        [MinLength(3, ErrorMessage = "Category Name must be at least 3 characters long")]
        [RegularExpression(@"[A-Za-z0-9À-ỹ\s]+", ErrorMessage = "Category Name cannot contain numbers or special characters")]
        public string CategoryName { get; set; }



    }
}
