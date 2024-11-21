using CookingBlogService.Data;
using CookingBlogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingBlogService.Services
{
    public class RecipeService
    {
        private readonly RecipeDbContext _context;

        public RecipeService(RecipeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetRecipes()
        {
            return await _context.Recipes.ToListAsync();
        }

        public async Task<Recipe?> GetRecipeById(int id)
        {
            return await _context.Recipes.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe> AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task UpdateRecipe(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<Recipe>> GetRecipesByOwner(string ownerId)
        {
            return await _context.Recipes
                .Where(r => r.OwnerId == ownerId)
                .ToListAsync();
        }
        
        // public async Task DeleteRecipesByOwner(string ownerId)
        // {
        //     var recipes = await GetRecipesByOwner(ownerId);
        //
        //     if (recipes.Any())
        //     {
        //         _context.Recipes.RemoveRange(recipes);
        //         await _context.SaveChangesAsync();
        //     }
        // }
        
        public async Task<bool> DeleteRecipesByOwner(string ownerId)
        {
            var recipes = await GetRecipesByOwner(ownerId);

            if (!recipes.Any()) return false;

            _context.Recipes.RemoveRange(recipes);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting recipes: {ex.Message}");
                throw new InvalidOperationException("Failed to delete recipes.");
            }
        }
        
        public async Task<bool> RestoreRecipes(List<Recipe> recipes)
        {
            try
            {
                _context.Recipes.AddRange(recipes);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring recipes: {ex.Message}");
                return false;
            }
        }
    }
}