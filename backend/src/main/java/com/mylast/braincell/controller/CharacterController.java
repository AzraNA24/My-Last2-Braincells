package com.mylast.braincell.controller;

import com.mylast.braincell.model.Character;
import com.mylast.braincell.model.CustomItem;
import com.mylast.braincell.model.CustomType;
import com.mylast.braincell.repository.CharacterRepository;
import com.mylast.braincell.repository.CustomItemRepository;
import com.mylast.braincell.repository.CustomTypeRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/characters")
public class CharacterController {
    private final CharacterRepository characterRepository;
    private final CustomTypeRepository customTypeRepository;
    private final CustomItemRepository customItemRepository;

    @Autowired
    public CharacterController(CharacterRepository characterRepository,
                               CustomTypeRepository customTypeRepository,
                               CustomItemRepository customItemRepository) {
        this.characterRepository = characterRepository;
        this.customTypeRepository = customTypeRepository;
        this.customItemRepository = customItemRepository;
    }

    @PostMapping("/create")
    public BaseResponse<Character> createCharacter(
            @RequestParam String name,
            @RequestParam String imagePath) {

        if (characterRepository.findByName(name) != null) {
            return new BaseResponse<>(false, "Character name already exists", null);
        }

        Character newCharacter = new Character(name, imagePath);
        characterRepository.save(newCharacter);
        return new BaseResponse<>(true, "Character created", newCharacter);
    }

    @PutMapping("/update/{id}")
    public BaseResponse<Character> updateCharacter(
            @PathVariable Long id,
            @RequestParam(required = false) String name,
            @RequestParam(required = false) String imagePath) {

        Character character = characterRepository.findById(id).orElse(null);
        if (character == null) {
            return new BaseResponse<>(false, "Character not found", null);
        }

        if (name != null) character.setName(name);
        if (imagePath != null) character.setImagePath(imagePath);

        characterRepository.save(character);
        return new BaseResponse<>(true, "Character updated", character);
    }

    @GetMapping("/")
    public BaseResponse<List<Character>> getAllCharacters() {
        return new BaseResponse<>(true, "Characters retrieved", characterRepository.findAll());
    }

    @GetMapping("/{id}/customizations")
    public BaseResponse<List<CustomType>> getCharacterCustomizations(@PathVariable Long id) {
        Character character = characterRepository.findById(id).orElse(null);
        if (character == null) {
            return new BaseResponse<>(false, "Character not found", null);
        }
        return new BaseResponse<>(true, "Custom types retrieved",
                customTypeRepository.findByCharacter(character));
    }

    @GetMapping("/custom-items/{typeId}")
    public BaseResponse<List<CustomItem>> getCustomItemsByType(@PathVariable Long typeId) {
        CustomType customType = customTypeRepository.findById(typeId).orElse(null);
        if (customType == null) {
            return new BaseResponse<>(false, "Custom type not found", null);
        }
        return new BaseResponse<>(true, "Custom items retrieved",
                customItemRepository.findByCustomType(customType));
    }
}