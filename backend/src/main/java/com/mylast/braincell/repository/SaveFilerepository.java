package com.mylast.braincell.repository;

import com.mylast.braincell.model.SaveFile;
import com.mylast.braincell.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface SaveFilerepository extends JpaRepository<SaveFile, Long> {
    List<SaveFile> findByUser(User user);
    SaveFile findByUserAndSlot(User user, int slot);
    // Return list (untuk multiple slots)
    List<SaveFile> findByUser_Id(Long userId);
    SaveFile findByUser_IdAndSlot(Long userId, int slot);
}