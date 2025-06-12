package io.github.my_last_2_braincells;

import com.badlogic.gdx.*;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.OrthographicCamera;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.Sprite;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.scenes.scene2d.ui.Skin;
import com.badlogic.gdx.utils.ScreenUtils;
import com.badlogic.gdx.utils.viewport.ExtendViewport;
import com.badlogic.gdx.utils.viewport.FitViewport;
import com.badlogic.gdx.utils.viewport.ScreenViewport;
import com.badlogic.gdx.utils.viewport.Viewport;
import io.github.my_last_2_braincells.Screen.MainMenuScreen;

/** {@link com.badlogic.gdx.ApplicationListener} implementation shared by all platforms. */
public class Main extends Game {
    public SpriteBatch batch; // Bisa diakses oleh semua screen
    public Skin skin; // Skin UI yang bisa dipakai oleh semua screen

    @Override
    public void create() {
        batch = new SpriteBatch();

        // Load UI skin (gunakan skin default atau custom)
        skin = new Skin(Gdx.files.internal("ui/quantum-horizon/skin/quantum-horizon-ui.json"));

        // Set screen awal ke menu
        setScreen(new MainMenuScreen(this));
    }

    @Override
    public void dispose() {
        super.dispose();
        batch.dispose();
        skin.dispose();
    }
}
