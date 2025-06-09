package com.mylast.braincell.controller;

import com.mylast.braincell.model.Achievement;
import com.mylast.braincell.model.User;
import com.mylast.braincell.model.UserAchievement;
import com.mylast.braincell.repository.AchievementRepository;
import com.mylast.braincell.repository.UserAchievementRepository;
import com.mylast.braincell.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/achievements")
public class AchievementController {
    private final AchievementRepository achievementRepository;
    private final UserRepository userRepository;
    private final UserAchievementRepository userAchievementRepository;

    @Autowired
    public AchievementController(AchievementRepository achievementRepository,
                                 UserRepository userRepository,
                                 UserAchievementRepository userAchievementRepository) {
        this.achievementRepository = achievementRepository;
        this.userRepository = userRepository;
        this.userAchievementRepository = userAchievementRepository;
    }

    @GetMapping("/")
    public BaseResponse<List<Achievement>> getAllAchievements() {
        return new BaseResponse<>(true, "Achievements retrieved", achievementRepository.findAll());
    }

    @GetMapping("/user/{userId}")
    public BaseResponse<List<UserAchievement>> getUserAchievements(@PathVariable Long userId) {
        User user = userRepository.findById(userId).orElse(null);
        if (user == null) {
            return new BaseResponse<>(false, "User not found", null);
        }
        return new BaseResponse<>(true, "User achievements retrieved",
                userAchievementRepository.findByUser(user));
    }

    @PostMapping("/unlock")
    public BaseResponse<UserAchievement> unlockAchievement(@RequestParam Long userId,
                                                           @RequestParam Long achievementId) {
        User user = userRepository.findById(userId).orElse(null);
        Achievement achievement = achievementRepository.findById(achievementId).orElse(null);

        if (user == null || achievement == null) {
            return new BaseResponse<>(false, "User or achievement not found", null);
        }

        if (userAchievementRepository.existsByUserAndAchievement(user, achievement)) {
            return new BaseResponse<>(false, "Achievement already unlocked", null);
        }

        UserAchievement userAchievement = new UserAchievement(user, achievement);
        userAchievementRepository.save(userAchievement);
        return new BaseResponse<>(true, "Achievement unlocked", userAchievement);
    }

    @PostMapping("/create")
    public BaseResponse<Achievement> createAchievement(
            @RequestParam String name,
            @RequestParam String imagePath,
            @RequestParam (required = false) String description ) {
        if (name == null || name.isEmpty()) {
             return new BaseResponse<>(false, "Achievement name cannot be empty", null);
        }
        if (achievementRepository.findByName(name) != null) {
            return new BaseResponse<>(false, "Achievement with this name already exists", null);
        }

        Achievement newAchievement = new Achievement(name, imagePath, description);
        achievementRepository.save(newAchievement);

        return new BaseResponse<>(true, "Achievement created successfully", newAchievement);
    }
    @PutMapping("/update/{id}")
    public BaseResponse<Achievement> updateAchievement(
            @PathVariable Long id,
            @RequestParam(required = false) String name,
            @RequestParam(required = false) String imagePath,
            @RequestParam(required = false) String description) {

        Achievement achievement = achievementRepository.findById(id).orElse(null);
        if (achievement == null) {
            return new BaseResponse<>(false, "Achievement not found", null);
        }

        if (name != null) achievement.setName(name);
        if (imagePath != null) achievement.setImagePath(imagePath);
        if (description != null) achievement.setDescription(description);

        achievementRepository.save(achievement);
        return new BaseResponse<>(true, "Achievement updated", achievement);
    }
}