namespace CookingBlogService.Models;

public class Recipe
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? OwnerId { get; set; }
}
