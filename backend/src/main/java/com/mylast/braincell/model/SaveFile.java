package com.mylast.braincell.model;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "save_files")
public class SaveFile extends Serializable {
    @ManyToOne
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @Column(name = "slot", nullable = false)
    private int slot; // 1-3

    @Column(name = "created_at", nullable = false)
    private LocalDateTime createdAt;

    // Constructors
    public SaveFile() {
        this.createdAt = LocalDateTime.now();
    }

    public SaveFile(User user, int slot) {
        this.user = user;
        this.slot = slot;
        this.createdAt = LocalDateTime.now();
    }

    // Getters and Setters
    public User getUser() {
        return user;
    }

    public void setUser(User user) {
        this.user = user;
    }

    public int getSlot() {
        return slot;
    }

    public void setSlot(int slot) {
        this.slot = slot;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}