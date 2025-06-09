package com.mylast.braincell.model;

import jakarta.persistence.*;

@Entity
@Table(name = "character")
public class Character extends Serializable {
    @Column(name = "name", nullable = false)
    private String name;

    @Column(name = "image_path", nullable = false)
    private String imagePath;

    // Constructors, Getters and Setters
    public Character() {}

    public Character(String name, String imagePath) {
        this.name = name;
        this.imagePath = imagePath;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getImagePath() {
        return imagePath;
    }

    public void setImagePath(String imagePath) {
        this.imagePath = imagePath;
    }
}