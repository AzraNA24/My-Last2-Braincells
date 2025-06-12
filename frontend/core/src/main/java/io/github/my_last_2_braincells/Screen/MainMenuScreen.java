package io.github.my_last_2_braincells.Screen;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Screen;
import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.badlogic.gdx.scenes.scene2d.InputEvent;
import com.badlogic.gdx.scenes.scene2d.Stage;
import com.badlogic.gdx.scenes.scene2d.ui.*;
import com.badlogic.gdx.scenes.scene2d.utils.ClickListener;
import com.badlogic.gdx.scenes.scene2d.utils.TextureRegionDrawable;
import com.badlogic.gdx.utils.Align;
import com.badlogic.gdx.utils.Scaling;
import com.badlogic.gdx.utils.Json;
import com.badlogic.gdx.utils.Json.*;
import com.badlogic.gdx.utils.JsonValue;
import com.badlogic.gdx.utils.JsonWriter;
import com.badlogic.gdx.utils.viewport.FitViewport;
import io.github.my_last_2_braincells.Main;
import io.github.my_last_2_braincells.auth.LoginRequest;
import io.github.my_last_2_braincells.auth.SignupRequest;

import java.io.IOException;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.StandardCharsets;

public class MainMenuScreen implements Screen {
    private final Main game;
    private Stage stage;
    private Texture logoTexture;
    private Color backgroundColor = new Color(0.1f, 0.1f, 0.2f, 1); // Default dark blue
    private Dialog loginDialog;
    private Dialog signupDialog;

    public MainMenuScreen(Main game) {
        this.game = game;
    }

    public void setBackgroundColor(float r, float g, float b, float a) {
        backgroundColor.set(r, g, b, a);
    }

    @Override
    public void show() {
        stage = new Stage(new FitViewport(800, 480));
        Gdx.input.setInputProcessor(stage);

        setBackgroundColor(33f/255f, 23f/255f, 56f/255f, 1.0f);

        // Load assets
        logoTexture = new Texture(Gdx.files.internal("ui/title.png"));

        // Main table untuk layout
        Table mainTable = new Table();
        mainTable.setFillParent(true);
        stage.addActor(mainTable);

        // Account buttons
        Table accountTable = new Table();
        TextButton loginButton = new TextButton("Login", game.skin);
        TextButton signupButton = new TextButton("Sign Up", game.skin);

        loginButton.getLabel().setFontScale(0.8f);
        signupButton.getLabel().setFontScale(0.8f);

        accountTable.add(loginButton).padRight(10).width(100).height(40);
        accountTable.add(signupButton).width(100).height(40);

        // Left menu (Start, Achievement)
        Table leftMenu = new Table();
        leftMenu.defaults().pad(10);

        TextButton startButton = new TextButton("Start", game.skin);
        TextButton achievementButton = new TextButton("Achievement", game.skin);

        startButton.getLabel().setFontScale(1f);
        achievementButton.getLabel().setFontScale(1f);

        leftMenu.add(startButton).width(250).height(70).padBottom(20).row();
        leftMenu.add(achievementButton).width(250).height(70).row();

        // Right side logo
        Image logoImage = new Image(logoTexture);
        logoImage.setScaling(Scaling.fit);

        // Layout
        mainTable.top().left();
        mainTable.add(accountTable).padTop(20).padLeft(20).colspan(2).row();
        mainTable.add(leftMenu).expand().bottom().padBottom(150).padLeft(50);
        mainTable.add(logoImage).expand().center().padRight(50).padBottom(50);

        // Initialize dialogs
        createLoginDialog();
        createSignupDialog();

        // Event listeners
        startButton.addListener(new ClickListener() {
            @Override
            public void clicked(InputEvent event, float x, float y) {
                game.setScreen(new GameScreen(game));
            }
        });

        achievementButton.addListener(new ClickListener() {
            @Override
            public void clicked(InputEvent event, float x, float y) {
                game.setScreen(new AchievementScreen(game));
            }
        });

        loginButton.addListener(new ClickListener() {
            @Override
            public void clicked(InputEvent event, float x, float y) {
                loginDialog.show(stage);
            }
        });

        signupButton.addListener(new ClickListener() {
            @Override
            public void clicked(InputEvent event, float x, float y) {
                signupDialog.show(stage);
            }
        });
    }

    private void createLoginDialog() {
        loginDialog = new Dialog("Login", game.skin) {
            @Override
            protected void result(Object object) {
                if ((Boolean) object) {
                    TextField usernameField = (TextField) this.findActor("username");
                    TextField passwordField = (TextField) this.findActor("password");

                    final String username = usernameField.getText();
                    final String password = passwordField.getText();

                    if (username.isEmpty() || password.isEmpty()) {
                        showErrorDialog("Harap isi semua field!");
                        return;
                    }

                    new Thread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                Json json = new Json();
                                String jsonData = json.toJson(new LoginRequest(username, password));

                                final boolean success = makeApiCall(
                                    "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/users/login",
                                    jsonData
                                );

                                Gdx.app.postRunnable(new Runnable() {
                                    @Override
                                    public void run() {
                                        if (success) {
                                            showSuccessDialog("Login berhasil!");
                                        } else {
                                            showErrorDialog("Login gagal.");
                                        }
                                    }
                                });
                            } catch (final Exception e) {
                                Gdx.app.postRunnable(new Runnable() {
                                    @Override
                                    public void run() {
                                        showErrorDialog("Error: " + e.getMessage());
                                    }
                                });
                            }
                        }
                    }).start();
                }
                this.hide();
            }
        };

        loginDialog.getContentTable().defaults().pad(10);

        TextField usernameField = new TextField("", game.skin);
        usernameField.setName("username");
        TextField passwordField = new TextField("", game.skin);
        passwordField.setPasswordMode(true);
        passwordField.setPasswordCharacter('*');
        passwordField.setName("password");

        loginDialog.getContentTable().add("Username:").padRight(10);
        loginDialog.getContentTable().add(usernameField).width(200).row();
        loginDialog.getContentTable().add("Password:").padRight(10);
        loginDialog.getContentTable().add(passwordField).width(200).row();

        loginDialog.button("Login", true);
        loginDialog.button("Cancel", false);
    }

    private void createSignupDialog() {
        signupDialog = new Dialog("Sign Up", game.skin) {
            @Override
            protected void result(Object object) {
                if ((Boolean) object) {
                    TextField emailField = (TextField) this.findActor("email");
                    TextField usernameField = (TextField) this.findActor("username");
                    TextField passwordField = (TextField) this.findActor("password");

                    final String email = emailField.getText();
                    final String username = usernameField.getText();
                    final String password = passwordField.getText();

                    if (email.isEmpty() || username.isEmpty() || password.isEmpty()) {
                        showErrorDialog("Harap isi semua field!");
                        return;
                    }

                    new Thread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                Json json = new Json();
                                String jsonData = json.toJson(new SignupRequest(email, username, password));

                                final boolean success = makeApiCall(
                                    "https://my-last2-braincells-backend-production-c9ac.up.railway.app/api/users/register",
                                    jsonData
                                );

                                Gdx.app.postRunnable(new Runnable() {
                                    @Override
                                    public void run() {
                                        if (success) {
                                            showSuccessDialog("Signup berhasil!");
                                        } else {
                                            showErrorDialog("Signup gagal.");
                                        }
                                    }
                                });
                            } catch (final Exception e) {
                                Gdx.app.postRunnable(new Runnable() {
                                    @Override
                                    public void run() {
                                        showErrorDialog("Error: " + e.getMessage());
                                    }
                                });
                            }
                        }
                    }).start();
                }
                this.hide();
            }
        };

        signupDialog.getContentTable().defaults().pad(10);

        TextField emailField = new TextField("", game.skin);
        emailField.setName("email");
        TextField usernameField = new TextField("", game.skin);
        usernameField.setName("username");
        TextField passwordField = new TextField("", game.skin);
        passwordField.setPasswordMode(true);
        passwordField.setPasswordCharacter('*');
        passwordField.setName("password");

        signupDialog.getContentTable().add("Email:").padRight(10);
        signupDialog.getContentTable().add(emailField).width(200).row();
        signupDialog.getContentTable().add("Username:").padRight(10);
        signupDialog.getContentTable().add(usernameField).width(200).row();
        signupDialog.getContentTable().add("Password:").padRight(10);
        signupDialog.getContentTable().add(passwordField).width(200).row();

        signupDialog.button("Sign Up", true);
        signupDialog.button("Cancel", false);
    }

    private boolean makeApiCall(String urlString, String jsonInput) {
        HttpURLConnection connection = null;
        OutputStream os = null;
        try {
            URL url = new URL(urlString);
            connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod("POST");
            connection.setRequestProperty("Content-Type", "application/json");
            connection.setRequestProperty("Accept", "application/json");
            connection.setDoOutput(true);

            os = connection.getOutputStream();
            byte[] input = jsonInput.getBytes("UTF-8");
            os.write(input, 0, input.length);
            os.flush();

            int responseCode = connection.getResponseCode();
            return responseCode == HttpURLConnection.HTTP_OK || responseCode == HttpURLConnection.HTTP_CREATED;
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        } finally {
            if (os != null) {
                try { os.close(); } catch (IOException e) { e.printStackTrace(); }
            }
            if (connection != null) {
                connection.disconnect();
            }
        }
    }

    private void showErrorDialog(String message) {
        Dialog errorDialog = new Dialog("Error", game.skin);
        errorDialog.text(message);
        errorDialog.button("OK");
        errorDialog.show(stage);
    }

    private void showSuccessDialog(String message) {
        Dialog successDialog = new Dialog("Success", game.skin);
        successDialog.text(message);
        successDialog.button("OK");
        successDialog.show(stage);
    }

    @Override
    public void render(float delta) {
        Gdx.gl.glClearColor(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);
        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);

        stage.act(delta);
        stage.draw();
    }

    @Override
    public void resize(int width, int height) {
        stage.getViewport().update(width, height, true);
    }

    @Override
    public void pause() {}

    @Override
    public void resume() {}

    @Override
    public void hide() {}

    @Override
    public void dispose() {
        stage.dispose();
        logoTexture.dispose();
    }
}
