﻿using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Recipe
{
    public class CreateRecipeDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = default!;

        [Required(ErrorMessage = "Cook time is required.")]
        public TimeOnly CookTime { get; set; }

        [Required(ErrorMessage = "Serves is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Serves must be greater than 0.")]
        public int Serves { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "CategoryID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryID must be a positive integer.")]
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Ingredients are required.")]
        [MinLength(1, ErrorMessage = "At least one ingredient is required.")]
        public List<CreateIngredient> Ingredients { get; set; } = new List<CreateIngredient>();

        [Required(ErrorMessage = "Recipe steps are required.")]
        [MinLength(1, ErrorMessage = "At least one recipe step is required.")]
        public List<CreateRecipeSteps> RecipeSteps { get; set; } = new List<CreateRecipeSteps>();

        public List<int> RelativeProducts { get; set; } = new List<int>();
    }

    public class CreateIngredient
    {
        [Required(ErrorMessage = "Ingredient name is required.")]
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, float.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public float Quantity { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [MaxLength(50, ErrorMessage = "Unit cannot exceed 50 characters.")]
        public string Unit { get; set; } = default!;

        public int? ProductID { get; set; }
    }

    public class CreateRecipeSteps
    {
        [Required(ErrorMessage = "Step number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Step must be greater than 0.")]
        public int Step { get; set; }

        public IFormFile? ImageStep { get; set; }

        [Required(ErrorMessage = "Directions are required.")]
        [MaxLength(1000, ErrorMessage = "Directions cannot exceed 1000 characters.")]
        public string Directions { get; set; } = default!;
    }
}
