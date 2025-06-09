package com.mylast.braincell.repository;

import com.mylast.braincell.model.CustomType;
import com.mylast.braincell.model.User;
import com.mylast.braincell.model.Character;
import com.mylast.braincell.model.UserCharacterCustomization;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface UserCharacterCustomizationRepository extends JpaRepository<UserCharacterCustomization, Long> {

    List<UserCharacterCustomization> findByUserAndCharacter(User user, Character character);

    Optional<UserCharacterCustomization> findByUserAndCharacterAndCustomType(
            User user, Character character, CustomType customType);

    List<UserCharacterCustomization> findByUserIdAndCharacterId(Long userId, Long characterId);
}
