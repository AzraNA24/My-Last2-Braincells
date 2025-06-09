-- Table: users
CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Table: achievements
CREATE TABLE achievements (
    achieve_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    image_path TEXT,
    description TEXT
);

-- Table: character
CREATE TABLE character (
    character_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    image_path TEXT
);

-- Table: levels
CREATE TABLE levels (
    level_id SERIAL PRIMARY KEY,
    level INT NOT NULL,
    title TEXT NOT NULL,
    enemy_id INT NOT NULL,
);

-- Table: custom_type
CREATE TABLE custom_types (
    type_id SERIAL PRIMARY KEY,
    custom_type TEXT NOT NULL,
    character_id INT NOT NULL,
    FOREIGN KEY (character_id) REFERENCES characters(character_id)
);

-- Table: custom_items
CREATE TABLE custom_items (
    item_id SERIAL PRIMARY KEY,
    custom_type INT NOT NULL,
    name TEXT NOT NULL,
    image_path TEXT,
    FOREIGN KEY (custom_type) REFERENCES custom_type(id)
);

-- Table: save_files
CREATE TABLE save_files (
    save_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    slot INT NOT NULL CHECK (slot BETWEEN 1 AND 3),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- Table: user_achievements
CREATE TABLE user_achievements (
    user_achieve_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    achieve_id INT NOT NULL,
    unlocked_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (achieve_id) REFERENCES achievements(achieve_id),
    UNIQUE (user_id, achieve_id)
);

-- Table: level_progress
CREATE TABLE level_progress (
    progress_id SERIAL PRIMARY KEY,
    character_id INT NOT NULL,
    save_id INT NOT NULL,
    level INT NOT NULL,
    collect_earned INT NOT NULL CHECK (collect_earned BETWEEN 1 AND 4),
    best_time VARCHAR(10),
    attempts INT NOT NULL DEFAULT 0,
    iscompleted BOOLEAN NOT NULL DEFAULT false,
    FOREIGN KEY (character_id) REFERENCES character(id),
    FOREIGN KEY (save_id) REFERENCES save_files(save_id),
    FOREIGN KEY (level) REFERENCES levels(level_id)
);