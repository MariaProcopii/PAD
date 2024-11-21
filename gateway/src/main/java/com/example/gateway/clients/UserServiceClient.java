package com.example.gateway.clients;

import com.example.gateway.dto.UserDTO;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@FeignClient(name = "user-service", url = "http://my-gateway-app:8080")
public interface UserServiceClient {

    @GetMapping("/user/profile/info/{id}")
    ResponseEntity<UserDTO> getUserByIdResponse(@PathVariable("id") String userId);

    @DeleteMapping("/user/profile/delete/{id}")
    String deleteUserResponse(@PathVariable("id") String userId);

    @PostMapping("/user/profile/restore")
    String restoreUser(@RequestBody UserDTO userRestoreDTO);
}