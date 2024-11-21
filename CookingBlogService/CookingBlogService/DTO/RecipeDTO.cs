namespace CookingBlogService.DTO;

public class RecipeDTO
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? OwnerId { get; set; }
}