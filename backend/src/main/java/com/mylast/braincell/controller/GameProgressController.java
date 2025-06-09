package com.mylast.braincell.controller;

import com.mylast.braincell.model.*;
import com.mylast.braincell.model.Character;
import com.mylast.braincell.repository.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/progress")
public class GameProgressController {
    private final LevelProgressRepository levelProgressRepository;
    private final SaveFilerepository saveFileRepository;
    private final Levelrepository levelRepository;
    private final CharacterRepository characterRepository;

    @Autowired
    public GameProgressController(LevelProgressRepository levelProgressRepository,
                                  SaveFilerepository saveFileRepository,
                                  Levelrepository levelRepository,
                                  CharacterRepository characterRepository) {
        this.levelProgressRepository = levelProgressRepository;
        this.saveFileRepository = saveFileRepository;
        this.levelRepository = levelRepository;
        this.characterRepository = characterRepository;
    }

    @GetMapping("/save/{saveId}")
    public BaseResponse<List<LevelProgress>> getSaveProgress(@PathVariable Long saveId) {
        SaveFile saveFile = saveFileRepository.findById(saveId).orElse(null);
        if (saveFile == null) {
            return new BaseResponse<>(false, "Save file not found", null);
        }
        return new BaseResponse<>(true, "Progress retrieved",
                levelProgressRepository.findBySaveFile(saveFile));
    }

    @PostMapping("/update")
    public BaseResponse<LevelProgress> updateProgress(
            @RequestParam Long saveId,
            @RequestParam int levelNumber,
            @RequestParam Long characterId,
            @RequestParam int collectEarned,
            @RequestParam String bestTime,
            @RequestParam boolean isCompleted) {

        try {
            SaveFile saveFile = saveFileRepository.findById(saveId)
                    .orElseThrow(() -> new IllegalArgumentException("Save file not found"));
            Level level = levelRepository.findByLevel(levelNumber);
            if (level == null) {
                return new BaseResponse<>(false, "Level not found", null);
            }
            Character character = characterRepository.findById(characterId)
                    .orElseThrow(() -> new IllegalArgumentException("Character not found"));

            // Validasi collectEarned
            if (collectEarned < 1 || collectEarned > 4) {
                return new BaseResponse<>(false, "collectEarned must be between 1-4", null);
            }

            LevelProgress progress = levelProgressRepository.findBySaveFileAndLevel(saveFile, level);
            if (progress == null) {
                progress = new LevelProgress(character, saveFile, level);
            }

            progress.setCollectEarned(collectEarned);
            progress.setBestTime(bestTime);
            progress.setCompleted(isCompleted);
            progress.setAttempts(progress.getAttempts() + 1);

            levelProgressRepository.save(progress);
            return new BaseResponse<>(true, "Progress updated", progress);
        } catch (Exception e) {
            return new BaseResponse<>(false, "Error: " + e.getMessage(), null);
        }
    }
}