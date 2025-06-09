package com.mylast.braincell.model;

import jakarta.persistence.*;

@Entity
@Table(name = "custom_items")
public class CustomItem extends Serializable {
    @ManyToOne
    @JoinColumn(name = "custom_type", nullable = false)
    private CustomType customType;

    @Column(name = "name", nullable = false)
    private String name;

    @Column(name = "image_path", nullable = false)
    private String imagePath;

    // Constructors, Getters and Setters
    public CustomItem() {}

    public CustomItem(CustomType customType, String name, String imagePath) {
        this.customType = customType;
        this.name = name;
        this.imagePath = imagePath;
    }

    public CustomType getCustomType() {
        return customType;
    }

    public void setCustomType(CustomType customType) {
        this.customType = customType;
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