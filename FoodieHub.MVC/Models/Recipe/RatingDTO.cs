using System.ComponentModel.DataAnnotations;

public class RatingDTO
{
    [Required]
    [Range(1, 5)]
    public int RatingValue { get; set; }

    [Required]
    public int RecipeID { get; set; }
}
