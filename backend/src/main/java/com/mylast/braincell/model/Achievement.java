package com.mylast.braincell.model;

import jakarta.persistence.*;

@Entity
@Table(name = "achievements")
public class Achievement extends Serializable {
    @Column(name = "name", length = 50, nullable = false)
    private String name;

    @Column(name = "image_path", nullable = false)
    private String imagePath;

    @Column(name = "description")
    private String description;

    // Constructors, Getters and Setters
    public Achievement() {}

    public Achievement(String name, String imagePath, String description) {
        this.name = name;
        this.imagePath = imagePath;
        this.description = description;
    }

    // Getters and Setters
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

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}