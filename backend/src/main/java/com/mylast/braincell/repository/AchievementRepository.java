package com.mylast.braincell.repository;

import com.mylast.braincell.model.Achievement;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface AchievementRepository extends JpaRepository<Achievement, Long> {
    Achievement findByName(String name);

    List<Achievement> findByNameContainingIgnoreCase(String name);
}