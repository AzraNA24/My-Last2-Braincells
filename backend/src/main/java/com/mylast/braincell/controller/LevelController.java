package com.mylast.braincell.controller;

import com.mylast.braincell.model.Level;
import com.mylast.braincell.repository.Levelrepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/levels")
public class LevelController {
    private final Levelrepository levelRepository;

    @Autowired
    public LevelController(Levelrepository levelRepository) {
        this.levelRepository = levelRepository;
    }

    @PostMapping("/create")
    public BaseResponse<Level> createLevel(
            @RequestParam int levelNumber,
            @RequestParam String title) {

        if (levelRepository.findByLevel(levelNumber) != null) {
            return new BaseResponse<>(false, "Level number already exists", null);
        }

        Level newLevel = new Level(levelNumber, title);
        levelRepository.save(newLevel);
        return new BaseResponse<>(true, "Level created", newLevel);
    }

    // Update level
    @PutMapping("/update/{id}")
    public BaseResponse<Level> updateLevel(
            @PathVariable Long id,
            @RequestParam(required = false) Integer levelNumber,
            @RequestParam(required = false) String title) {

        Level level = levelRepository.findById(id).orElse(null);
        if (level == null) {
            return new BaseResponse<>(false, "Level not found", null);
        }

        if (levelNumber != null) level.setLevel(levelNumber);
        if (title != null) level.setTitle(title);

        levelRepository.save(level);
        return new BaseResponse<>(true, "Level updated", level);
    }

    // Get all levels
    @GetMapping("/")
    public BaseResponse<List<Level>> getAllLevels() {
        return new BaseResponse<>(true, "Levels retrieved", levelRepository.findAll());
    }
}
