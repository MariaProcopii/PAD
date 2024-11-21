package com.example.gateway.clients;

import com.example.gateway.dto.RecipeDTO;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@FeignClient(name = "cooking-blog-service", url = "http://my-gateway-app:8080")
public interface CookingBlogServiceClient {

    @GetMapping("/recipe/by-owner/{id}")
    List<RecipeDTO> getRecipesByOwnerResponse(@PathVariable("id") String ownerId);

    @DeleteMapping("/recipe/delete-by-owner/{id}")
    String deleteRecipesByOwnerResponse(@PathVariable("id") String ownerId);

    @PostMapping("/recipe/restore")
    String restoreRecipes(@RequestBody List<RecipeDTO> recipes);
}