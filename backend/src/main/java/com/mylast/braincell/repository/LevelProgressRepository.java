package com.mylast.braincell.repository;

import com.mylast.braincell.model.Level;
import com.mylast.braincell.model.LevelProgress;
import com.mylast.braincell.model.SaveFile;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;
import java.util.Optional;

public interface LevelProgressRepository extends JpaRepository<LevelProgress, Long> {
    List<LevelProgress> findBySaveFile(SaveFile saveFile);
    LevelProgress findBySaveFileAndLevel(SaveFile saveFile, Level level);
    Optional<LevelProgress> findBySaveFileIdAndLevelId(Long saveId, Long levelId);
}