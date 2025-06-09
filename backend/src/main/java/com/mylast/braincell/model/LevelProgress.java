package com.mylast.braincell.model;

import jakarta.persistence.*;

@Entity
@Table(name = "level_progress")
public class LevelProgress extends Serializable {
    @ManyToOne
    @JoinColumn(name = "character_id", nullable = false)
    private Character character;

    @ManyToOne
    @JoinColumn(name = "save_id", nullable = false)
    private SaveFile saveFile;

    @ManyToOne
    @JoinColumn(name = "level", nullable = false)
    private Level level;

    @Column(name = "collect_earned", nullable = false)
    private int collectEarned; // 1-4

    @Column(name = "best_time", length = 10)
    private String bestTime;

    @Column(name = "attempts", nullable = false)
    private int attempts;

    @Column(name = "iscompleted", nullable = false)
    private boolean isCompleted;

    // Constructors, Getters and Setters
    public LevelProgress() {
        this.collectEarned = 0;
        this.attempts = 0;
        this.isCompleted = false;
    }

    public LevelProgress(Character character, SaveFile saveFile, Level level) {
        this();
        this.character = character;
        this.saveFile = saveFile;
        this.level = level;
    }

    // Getters and Setters
    public Character getCharacter() {
        return character;
    }

    public void setCharacter(Character character) {
        this.character = character;
    }

    public SaveFile getSaveFile() {
        return saveFile;
    }

    public void setSaveFile(SaveFile saveFile) {
        this.saveFile = saveFile;
    }

    public Level getLevel() {
        return level;
    }

    public void setLevel(Level level) {
        this.level = level;
    }

    public int getCollectEarned() {
        return collectEarned;
    }

    public void setCollectEarned(int collectEarned) {
        this.collectEarned = collectEarned;
    }

    public String getBestTime() {
        return bestTime;
    }

    public void setBestTime(String bestTime) {
        this.bestTime = bestTime;
    }

    public int getAttempts() {
        return attempts;
    }

    public void setAttempts(int attempts) {
        this.attempts = attempts;
    }

    public boolean isCompleted() {
        return isCompleted;
    }

    public void setCompleted(boolean completed) {
        isCompleted = completed;
    }
}