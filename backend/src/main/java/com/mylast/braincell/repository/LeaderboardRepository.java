//package com.mylast.braincell.repository;
//
//import com.mylast.braincell.model.Leaderboard;
//import org.springframework.data.jpa.repository.JpaRepository;
//
//import java.util.List;
//
//public interface LeaderboardRepository extends JpaRepository <Leaderboard, Long> {
//    Leaderboard findById(Long id);
//
//    Leaderboard deleteById(Long id);
//
//    void deleteById(Long id);
//    List<Leaderboard> findByLevelOrderByScoreDesc(int level);
//}
