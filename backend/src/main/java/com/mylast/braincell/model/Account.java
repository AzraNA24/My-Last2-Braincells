//package com.mylast.braincell.model;
//
//import jakarta.persistence.Column;
//import jakarta.persistence.Entity;
//import jakarta.persistence.Table;
//
//@Entity
//@Table(name = "account")
//public class Account extends Serializable {
//    @Column(name = "username", unique = true, nullable = false)
//    String username;
//
//    @Column(name = "password", nullable = false)
//    String password;
//
//    public Account() {
//        this.username = "";
//        this.password = "";
//    }
//
//    public Account(String username, String password) {
//        this.username = username;
//        this.password = password;
//    }
//    public String getUsername() {
//        return username;
//    }
//    public void setUsername(String username) {
//        this.username = username;
//    }
//    public String getPassword() {
//        return password;
//    }
//    public void setPassword(String password) {
//        this.password = password;
//    }
//
//}