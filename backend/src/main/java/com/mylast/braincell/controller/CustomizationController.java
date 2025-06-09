package com.mylast.braincell.controller;

import com.mylast.braincell.model.*;
import com.mylast.braincell.model.Character;
import com.mylast.braincell.model.UserCharacterCustomization;
import com.mylast.braincell.repository.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;

@RestController
@RequestMapping("/api/customizations")
public class CustomizationController {

    @Autowired
    private UserCharacterCustomizationRepository customizationRepository;

    @Autowired
    private CustomItemRepository customItemRepository;

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private CharacterRepository characterRepository;

    @Autowired
    private CustomTypeRepository customTypeRepository;

    @PostMapping("/create")
    public BaseResponse<UserCharacterCustomization> createCustomization(
            @RequestParam Long userId,
            @RequestParam Long characterId,
            @RequestParam Long customTypeId,
            @RequestParam Long customItemId) {

        // Validasi semua entity exist
        User user = userRepository.findById(userId).orElse(null);
        Character character = characterRepository.findById(characterId).orElse(null);
        CustomType customType = customTypeRepository.findById(customTypeId).orElse(null);
        CustomItem customItem = customItemRepository.findById(customItemId).orElse(null);

        if (user == null || character == null || customType == null || customItem == null) {
            return new BaseResponse<>(false, "Invalid user, character, type, or item", null);
        }

        // Validasi custom item termasuk dalam type dan character yang benar
        if (!customItem.getCustomType().getId().equals(customTypeId)) {
            return new BaseResponse<>(false, "Item doesn't belong to this custom type", null);
        }

        if (!customType.getCharacter().getId().equals(characterId)) {
            return new BaseResponse<>(false, "Custom type doesn't belong to this character", null);
        }

        // Cek customization untuk type ini
        Optional<UserCharacterCustomization> existing = customizationRepository
                .findByUserAndCharacterAndCustomType(user, character, customType);

        if (existing.isPresent()) {
            return new BaseResponse<>(false, "Customization already exists for this type", null);
        }

        // Buat customization
        UserCharacterCustomization newCustomization = new UserCharacterCustomization(
                user, character, customType, customItem);

        customizationRepository.save(newCustomization);
        return new BaseResponse<>(true, "Customization created", newCustomization);
    }

    @PutMapping("/update/{customizationId}")
    public BaseResponse<UserCharacterCustomization> updateCustomization(
            @PathVariable Long customizationId,
            @RequestParam Long customItemId) {

        // Cari customization yang ada
        UserCharacterCustomization customization = customizationRepository
                .findById(customizationId)
                .orElse(null);

        if (customization == null) {
            return new BaseResponse<>(false, "Customization not found", null);
        }

        // Validasi custom item baru
        CustomItem newItem = customItemRepository.findById(customItemId).orElse(null);
        if (newItem == null) {
            return new BaseResponse<>(false, "Custom item not found", null);
        }

        // Pastikan item baru termasuk dalam custom type yang sama
        if (!newItem.getCustomType().getId().equals(customization.getCustomType().getId())) {
            return new BaseResponse<>(false, "Item must be in the same category", null);
        }

        // Update
        customization.setCustomItem(newItem);
        customizationRepository.save(customization);

        return new BaseResponse<>(true, "Customization updated", customization);
    }

    // Get customizations
    @GetMapping("/user/{userId}/character/{characterId}")
    public BaseResponse<List<UserCharacterCustomization>> getCustomizations(
            @PathVariable Long userId,
            @PathVariable Long characterId) {

        if (!userRepository.existsById(userId) || !characterRepository.existsById(characterId)) {
            return new BaseResponse<>(false, "User or character not found", null);
        }

        List<UserCharacterCustomization> customizations = customizationRepository
                .findByUserIdAndCharacterId(userId, characterId);

        return new BaseResponse<>(true, "Customizations retrieved", customizations);
    }
}
