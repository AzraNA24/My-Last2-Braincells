//package com.mylast.braincell.model;
//
//import jakarta.persistence.*;
//
//@Entity
//@Table(name = "Leaderboard")
//public class Leaderboard extends Serializable {
//    @Column(name = "level", nullable = false)
//    int level;
//
//    @Column(name = "score", nullable = false)
//    int score;
//
//    @ManyToOne
//    @JoinColumn(name = "account_id", referencedColumnName = "id", nullable = false)
//    Account account;
//
//    public Leaderboard() {
//        this.level = 0;
//        this.score = 0;
//        this.account = new Account();
//    }
//
//    public Leaderboard(int level, int score, Account account) {
//        this.level = level;
//        this.score = score;
//        this.account = account;
//    }
//
//    public int getLevel() {
//        return level;
//    }
//
//    public void setLevel(int level) {
//        this.level = level;
//    }
//
//    public int getScore() {
//        return score;
//    }
//
//    public void setScore(int score) {
//        this.score = score;
//    }
//
//    public Account getAccount() {
//        return account;
//    }
//
//    public void setAccount(Account account) {
//        this.account = account;
//    }
//}
