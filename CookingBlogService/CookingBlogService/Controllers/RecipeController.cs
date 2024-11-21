using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using CookingBlogService.DTO;
using CookingBlogService.Services;
using CookingBlogService.Models;
using Newtonsoft.Json;

namespace CookingBlogService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService _recipeService;
        private readonly HttpClient _httpClient;

        public RecipeController(RecipeService recipeService, HttpClient httpClient)
        {
            _recipeService = recipeService;
            _httpClient = httpClient;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllRecipes()
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            
            var recipes = await _recipeService.GetRecipes();
            var result = new List<RecipeFullInfoDTO>();
            
            foreach (var recipe in recipes)
            {
                var user = await GetUserDetails(recipe.OwnerId, accessToken);
                if (user == null)
                {
                    return Unauthorized("Invalid token or user not found.");
                }
            
                result.Add(new RecipeFullInfoDTO()
                {
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Username = user.Username,
                    Email = user.Email
                });
            }
            
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var recipe = await _recipeService.GetRecipeById(id);
            if (recipe == null) return NotFound("Recipe not found.");

            var user = await GetUserDetails(recipe.OwnerId, accessToken);
            if (user == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }

            return Ok(new
            {
                recipe.Id,
                recipe.Title,
                Author = new
                {
                    user.Username,
                    user.Email
                },
                recipe.Description
            });
        }
        
        [HttpPost("post/{ownerId}")]
        public async Task<IActionResult> PostRecipe(string ownerId, [FromBody] CreateRecipeDTO recipe)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var user = await GetUserDetails(ownerId, accessToken);
            if (user == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }

            var newRecipe = new Recipe()
            {
                Title = recipe.Title,
                Description = recipe.Description,
                OwnerId = ownerId
            };
            var createdRecipe = await _recipeService.AddRecipe(newRecipe);

            return CreatedAtAction(nameof(GetRecipeById), new { id = createdRecipe.Id }, createdRecipe);
        }


        [HttpPut("edit/{recipeId}")]
        public async Task<IActionResult> EditRecipe(int recipeId, [FromBody] RecipeDTO updatedRecipe)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var recipe = await _recipeService.GetRecipeById(recipeId);
            if (recipe == null) return NotFound("Recipe not found.");

            var user = await GetUserDetails(recipe.OwnerId, accessToken);
            if (user == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }

            recipe.Title = updatedRecipe.Title;
            recipe.Description = updatedRecipe.Description;
            await _recipeService.UpdateRecipe(recipe);

            return Ok(recipe);
        }
        
        private async Task<UserInfoDTO> GetUserDetails(string userId, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync($"http://my-gateway-app:8080/user/profile/info/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserInfoDTO>(json);
            }

            return null;
        }
        
        [HttpGet("by-owner/{ownerId}")]
        public async Task<IActionResult> GetRecipesByOwner(string ownerId)
        {
            try
            {
                var recipes = await _recipeService.GetRecipesByOwner(ownerId);

                if (recipes == null || !recipes.Any())
                {
                    return NotFound("No recipes found for the specified owner.");
                }

                var recipeDtos = recipes.Select(r => new RecipeDTO
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    OwnerId = r.OwnerId
                }).ToList();

                return Ok(recipeDtos);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        
        [HttpDelete("delete-by-owner/{ownerId}")]
        public async Task<IActionResult> DeleteRecipesByOwner(string ownerId)
        {
            try
            {
                var result = await _recipeService.DeleteRecipesByOwner(ownerId);
                if (!result) return NotFound("No recipes found for this owner.");

                return Ok(ownerId);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        
        [HttpPost("restore")]
        public async Task<IActionResult> RestoreRecipes([FromBody] List<RecipeDTO> recipes)
        {
            var recipesToRestore = recipes.Select(r => new Recipe
            {
                Title = r.Title,
                Description = r.Description,
                OwnerId = r.OwnerId
            }).ToList();

            var success = await _recipeService.RestoreRecipes(recipesToRestore);

            if (!success)
            {
                return StatusCode(500);
            }

            return Ok("Recipes restored successfully.");
        }

    }
}