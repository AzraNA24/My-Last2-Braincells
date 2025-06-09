package com.mylast.braincell.repository;

import com.mylast.braincell.model.CustomType;
import com.mylast.braincell.model.CustomItem;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface CustomItemRepository extends JpaRepository<CustomItem, Long> {
    List<CustomItem> findByCustomType(CustomType customType);
}