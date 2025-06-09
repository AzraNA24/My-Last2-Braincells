package com.mylast.braincell.repository;

import com.mylast.braincell.model.Character;
import com.mylast.braincell.model.CustomType;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface CustomTypeRepository extends JpaRepository<CustomType, Long> {
    List<CustomType> findByCharacter(Character character);
}