package com.example.gateway.controllers;

import com.example.gateway.clients.CookingBlogServiceClient;
import com.example.gateway.clients.UserServiceClient;
import com.example.gateway.dto.RecipeDTO;
import com.example.gateway.dto.UserDTO;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;
import java.util.List;

@RestController
@RequestMapping("/saga")
public class SagaController {

    private static final Logger logger = LoggerFactory.getLogger(SagaController.class);

    private final UserServiceClient userServiceClient;
    private final CookingBlogServiceClient cookingBlogServiceClient;

    public SagaController(UserServiceClient userServiceClient, CookingBlogServiceClient cookingBlogServiceClient) {
        this.userServiceClient = userServiceClient;
        this.cookingBlogServiceClient = cookingBlogServiceClient;
    }

    @DeleteMapping("/delete-user/{userId}")
    public ResponseEntity<String> deleteUserCascade(@PathVariable String userId) {
        List<String> sagaLog = new ArrayList<>();
        List<RecipeDTO> recipesBackup = new ArrayList<>();
        UserDTO userBackup = null;

        logger.info("Starting deleteUserCascade saga for userId: {}", userId);

        try {
            UserDTO userResponse = userServiceClient.getUserByIdResponse(userId).getBody();
            if (userResponse == null) {
                logger.error("Failed to fetch user details for userId: {}", userId);
//                throw new RuntimeException("Failed to fetch user details.");
            }
            userBackup = userResponse;
            sagaLog.add("UserFetched");
            logger.info("User details fetched successfully for userId: {}", userId);

            String deleteUserResponse = userServiceClient.deleteUserResponse(userId);
            if (deleteUserResponse == null || deleteUserResponse.isEmpty()) {
                logger.error("Failed to delete user with userId: {}", userId);
                throw new RuntimeException("Failed to delete user.");
            }
            sagaLog.add("UserDeleted");
            logger.info("User deleted successfully with userId: {}", userId);

            List<RecipeDTO> recipesResponse = cookingBlogServiceClient.getRecipesByOwnerResponse(userId);
            if (!recipesResponse.isEmpty()) {
                recipesBackup = recipesResponse;
                sagaLog.add("RecipesFetched");
                logger.info("Recipes fetched successfully for userId: {}", userId);

//                String deleteRecipesResponse = cookingBlogServiceClient.deleteRecipesByOwnerResponse(userId);
                String deleteRecipesResponse = null; //To show how saga rollback works
                if (deleteRecipesResponse == null || deleteRecipesResponse.isEmpty()) {
                    logger.error("Failed to delete recipes for userId: {}", userId);
                    throw new RuntimeException("Failed to delete recipes.");
                }
                sagaLog.add("RecipesDeleted");
                logger.info("Recipes deleted successfully for userId: {}", userId);
            }

            logger.info("Saga completed successfully for userId: {}", userId);
            return ResponseEntity.ok("User and all related recipes deleted successfully.");
        } catch (Exception ex) {

            if (sagaLog.contains("UserDeleted")) {
                logger.debug("Rolling back user deletion for userId: {}", userId);
                UserDTO restoreUser = new UserDTO(userId, userBackup.getUsername(), userBackup.getUsername(), userBackup.getPassword());
                userServiceClient.restoreUser(restoreUser);
                logger.info("User restored successfully for userId: {}", userId);
            }

            if (sagaLog.contains("RecipesDeleted") && !recipesBackup.isEmpty()) {
                logger.debug("Rolling back recipe deletion for userId: {}", userId);
                cookingBlogServiceClient.restoreRecipes(recipesBackup);
                logger.info("Recipes restored successfully for userId: {}", userId);
            }

            logger.error("Rollback completed for userId: {}", userId);
            return ResponseEntity.status(200).body("An error occurred. Rollback executed. " + ex.getMessage());
        }
    }
}