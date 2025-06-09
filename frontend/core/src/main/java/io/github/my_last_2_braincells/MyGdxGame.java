package io.github.my_last_2_braincells;

import com.badlogic.gdx.ApplicationAdapter;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.OrthographicCamera;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.math.Rectangle;
import com.badlogic.gdx.utils.Array;
import com.badlogic.gdx.utils.TimeUtils;

public class MyGdxGame extends ApplicationAdapter {
    private SpriteBatch batch;
    private Texture playerTexture, coinTexture;
    private Rectangle player;
    private Array<Rectangle> coins;
    private OrthographicCamera camera;
    private int score;

    @Override
    public void create () {
        batch = new SpriteBatch();
        playerTexture = new Texture("player.png");
        coinTexture = new Texture("coin.png");

        camera = new OrthographicCamera();
        camera.setToOrtho(false, 800, 480);

        player = new Rectangle();
        player.x = 800 / 2f - 32;
        player.y = 20;
        player.width = 64;
        player.height = 64;

        coins = new Array<>();
        spawnCoin();
        score = 0;
    }

    private void spawnCoin() {
        Rectangle coin = new Rectangle();
        coin.x = (float)Math.random() * (800 - 32);
        coin.y = (float)Math.random() * (480 - 32);
        coin.width = 32;
        coin.height = 32;
        coins.add(coin);
    }

    @Override
    public void render () {
        // Update input
        if (Gdx.input.isKeyPressed(Input.Keys.LEFT)) player.x -= 200 * Gdx.graphics.getDeltaTime();
        if (Gdx.input.isKeyPressed(Input.Keys.RIGHT)) player.x += 200 * Gdx.graphics.getDeltaTime();
        if (Gdx.input.isKeyPressed(Input.Keys.UP)) player.y += 200 * Gdx.graphics.getDeltaTime();
        if (Gdx.input.isKeyPressed(Input.Keys.DOWN)) player.y -= 200 * Gdx.graphics.getDeltaTime();

        // Batasi agar tidak keluar layar
        player.x = Math.max(0, Math.min(player.x, 800 - player.width));
        player.y = Math.max(0, Math.min(player.y, 480 - player.height));

        // Periksa tabrakan
        for (int i = 0; i < coins.size; i++) {
            if (player.overlaps(coins.get(i))) {
                coins.removeIndex(i);
                score++;
                spawnCoin();
                break;
            }
        }

        // Gambar
        Gdx.gl.glClearColor(0, 0, 0.2f, 1);
        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);

        camera.update();
        batch.setProjectionMatrix(camera.combined);

        batch.begin();
        batch.draw(playerTexture, player.x, player.y);
        for (Rectangle coin : coins) {
            batch.draw(coinTexture, coin.x, coin.y);
        }
        batch.end();
    }

    @Override
    public void dispose () {
        batch.dispose();
        playerTexture.dispose();
        coinTexture.dispose();
    }
}
