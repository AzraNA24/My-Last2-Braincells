package com.mylast.braincell.model;

import jakarta.persistence.*;

@Entity
@Table(name = "custom_type")
public class CustomType extends Serializable {
    @Column(name = "custom_type", nullable = false)
    private String customType;

    @ManyToOne
    @JoinColumn(name = "character_id", nullable = false)
    private Character character;

    // Constructors, Getters and Setters
    public CustomType() {}

    public CustomType(String customType, Character character) {
        this.customType = customType;
        this.character = character;
    }

    public String getCustomType() {
        return customType;
    }

    public void setCustomType(String customType) {
        this.customType = customType;
    }

    public Character getCharacter() {
        return character;
    }

    public void setCharacter(Character character) {
        this.character = character;
    }
}