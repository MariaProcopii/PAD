using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
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
        public async Task<IActionResult> PostRecipe(string ownerId, [FromBody] RecipeDTO recipe)
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
            var response = await _httpClient.GetAsync($"http://localhost:5236/user/profile/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserInfoDTO>(json);
            }

            return null;
        }
    }
}