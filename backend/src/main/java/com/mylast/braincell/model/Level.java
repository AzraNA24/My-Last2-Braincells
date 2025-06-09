package com.mylast.braincell.model;

import jakarta.persistence.*;

@Entity
@Table(name = "levels")
public class Level extends Serializable {
    @Column(name = "level", nullable = false, unique = true)
    private int level;

    @Column(name = "title", nullable = false)
    private String title;

    // Constructors, Getters and Setters
    public Level() {}

    public Level(int level, String title) {
        this.level = level;
        this.title = title;
    }

    public int getLevel() {
        return level;
    }

    public void setLevel(int level) {
        this.level = level;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }
}