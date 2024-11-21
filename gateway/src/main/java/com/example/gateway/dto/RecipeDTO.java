package com.example.gateway.dto;


import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
public class RecipeDTO {
    private int id;
    private String title;
    private String description;
    private String ownerId;
}