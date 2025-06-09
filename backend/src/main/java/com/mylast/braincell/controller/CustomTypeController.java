package com.mylast.braincell.controller;

import com.mylast.braincell.model.CustomType;
import com.mylast.braincell.repository.CharacterRepository;
import com.mylast.braincell.repository.CustomTypeRepository;
import com.mylast.braincell.model.Character;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/custom-types")
public class CustomTypeController {
    private final CustomTypeRepository customTypeRepository;
    private final CharacterRepository characterRepository;

    @Autowired
    public CustomTypeController(CustomTypeRepository customTypeRepository,
                                CharacterRepository characterRepository) {
        this.customTypeRepository = customTypeRepository;
        this.characterRepository = characterRepository;
    }

    @PostMapping("/create")
    public BaseResponse<CustomType> createCustomType(
            @RequestParam String customType,
            @RequestParam Long characterId) {

        Character character = characterRepository.findById(characterId).orElse(null);
        if (character == null) {
            return new BaseResponse<>(false, "Character not found", null);
        }

        CustomType newType = new CustomType(customType, character);
        customTypeRepository.save(newType);
        return new BaseResponse<>(true, "Custom type created", newType);
    }

    // Get custom types by character
    @GetMapping("/character/{characterId}")
    public BaseResponse<List<CustomType>> getByCharacter(@PathVariable Long characterId) {
        Character character = characterRepository.findById(characterId).orElse(null);
        if (character == null) {
            return new BaseResponse<>(false, "Character not found", null);
        }

        return new BaseResponse<>(true, "Custom types retrieved",
                customTypeRepository.findByCharacter(character));
    }

    @GetMapping("/")
    public BaseResponse<List<CustomType>> getAll() {
        return new BaseResponse<>(true, "All custom types retrieved", customTypeRepository.findAll());
    }
}
