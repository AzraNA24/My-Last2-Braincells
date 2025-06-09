package com.mylast.braincell.model;

import jakarta.persistence.*;

@Entity
@Table(name = "customizations")
public class UserCharacterCustomization extends Serializable {

    @ManyToOne
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @ManyToOne
    @JoinColumn(name = "character_id", nullable = false)
    private Character character;

    @ManyToOne
    @JoinColumn(name = "custom_type_id", nullable = false)
    private CustomType customType;

    @ManyToOne
    @JoinColumn(name = "custom_item_id", nullable = false)
    private CustomItem customItem;



    public UserCharacterCustomization() {

    }

    public UserCharacterCustomization(User user, Character character,
                                      CustomType customType, CustomItem customItem) {
        this.user = user;
        this.character = character;
        this.customType = customType;
        this.customItem = customItem;
    }
    public User getUser() {
        return user;
    }
    public void setUser(User user) {
        this.user = user;
    }
    public Character getCharacter() {
        return character;
    }
    public void setCharacter(Character character) {
        this.character = character;
    }
    public CustomType getCustomType() {
        return customType;
    }
    public void setCustomType(CustomType customType) {
        this.customType = customType;
    }
    public CustomItem getCustomItem() {
        return customItem;
    }
    public void setCustomItem(CustomItem customItem) {
        this.customItem = customItem;
    }
}
