package com.mylast.braincell.repository;

import com.mylast.braincell.model.Level;
import org.springframework.data.jpa.repository.JpaRepository;

public interface Levelrepository extends JpaRepository<Level, Long> {
    Level findByLevel(int level);
}