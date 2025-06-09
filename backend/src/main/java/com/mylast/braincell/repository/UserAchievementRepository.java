package com.mylast.braincell.repository;

import com.mylast.braincell.model.Achievement;
import com.mylast.braincell.model.User;
import com.mylast.braincell.model.UserAchievement;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface UserAchievementRepository extends JpaRepository<UserAchievement, Long> {
    List<UserAchievement> findByUser(User user);
    boolean existsByUserAndAchievement(User user, Achievement achievement);
}