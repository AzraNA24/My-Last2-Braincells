package com.mylast.braincell.controller;

import com.mylast.braincell.model.SaveFile;
import com.mylast.braincell.model.User;
import com.mylast.braincell.repository.SaveFilerepository;
import com.mylast.braincell.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/saves")
public class SaveFileController {
    private final SaveFilerepository saveFileRepository;
    private final UserRepository userRepository;

    @Autowired
    public SaveFileController(SaveFilerepository saveFileRepository,
                              UserRepository userRepository) {
        this.saveFileRepository = saveFileRepository;
        this.userRepository = userRepository;
    }

    @GetMapping("/user/{userId}")
    public BaseResponse<List<SaveFile>> getUserSaves(@PathVariable Long userId) {
        User user = userRepository.findById(userId).orElse(null);
        if (user == null) {
            return new BaseResponse<>(false, "User not found", null);
        }
        return new BaseResponse<>(true, "Save files retrieved",
                saveFileRepository.findByUser(user));
    }

    @PostMapping("/create")
    public BaseResponse<SaveFile> createSave(@RequestParam Long userId,
                                             @RequestParam int slot) {
        if (slot < 1 || slot > 3) {
            return new BaseResponse<>(false, "Invalid slot number (must be 1-3)", null);
        }

        User user = userRepository.findById(userId).orElse(null);
        if (user == null) {
            return new BaseResponse<>(false, "User not found", null);
        }

        SaveFile existingSave = saveFileRepository.findByUserAndSlot(user, slot);
        if (existingSave != null) {
            return new BaseResponse<>(false, "Save slot already occupied", null);
        }

        SaveFile newSave = new SaveFile(user, slot);
        saveFileRepository.save(newSave);
        return new BaseResponse<>(true, "Save file created", newSave);
    }
}