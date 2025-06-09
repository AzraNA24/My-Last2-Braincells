package com.mylast.braincell.model;

import jakarta.persistence.*;

@MappedSuperclass
public class Serializable {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    public void setId(Long id) {
        this.id = id;
    }
    public Long getId() {
        return id;
    }
}
