package io.github.my_last_2_braincells.auth;

public class SignupRequest {
    public String email;
    public String username;
    public String password;

    public SignupRequest(String email, String username, String password) {
        this.email = email;
        this.username = username;
        this.password = password;
    }
}
