package com.mylast.braincell.repository;

import com.mylast.braincell.model.Character;
import org.springframework.data.jpa.repository.JpaRepository;

public interface CharacterRepository extends JpaRepository<Character, Long> {
    Character findByName(String name);
}