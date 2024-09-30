using CookingBlogService.Models;

namespace CookingBlogService.Data;

using Microsoft.EntityFrameworkCore;

public class RecipeDbContext : DbContext
{
    public DbSet<Recipe> Recipes { get; set; }

    public RecipeDbContext(DbContextOptions<RecipeDbContext> options) : base(options)
    {
    }
}
