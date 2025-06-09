package com.mylast.braincell.controller;

import com.mylast.braincell.model.User;
import com.mylast.braincell.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/users")
public class UserController {
    private final UserRepository userRepository;

    @Autowired
    public UserController(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @GetMapping("/{id}")
    public BaseResponse<User> getUserById(@PathVariable Long id) {
        User user = userRepository.findById(id).orElse(null);
        if (user == null) {
            return new BaseResponse<>(false, "User not found", null);
        }
        return new BaseResponse<>(true, "User found", user);
    }

    @GetMapping("/username/{username}")
    public BaseResponse<User> getUserByUsername(@PathVariable String username) {
        User user = userRepository.findByUsername(username);
        if (user == null) {
            return new BaseResponse<>(false, "User not found", null);
        }
        return new BaseResponse<>(true, "User found", user);
    }

    @PostMapping("/register")
    public BaseResponse<User> registerUser(@RequestParam String username,
                                           @RequestParam String email,
                                           @RequestParam String password) {
        if (userRepository.findByUsername(username) != null) {
            return new BaseResponse<>(false, "Username already exists", null);
        }
        if (userRepository.findByEmail(email) != null) {
            return new BaseResponse<>(false, "Email already registered", null);
        }

        User newUser = new User(username, email, password);
        userRepository.save(newUser);
        return new BaseResponse<>(true, "User registered successfully", newUser);
    }
}