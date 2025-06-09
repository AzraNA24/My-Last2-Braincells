//package com.mylast.braincell.controller;
//
//import com.mylast.braincell.model.Account;
//import com.mylast.braincell.repository.AccountRepository;
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.web.bind.annotation.*;
//
//import java.util.List;
//
//@RestController
//@RequestMapping("/account")
//public class AccountController {
//    private final AccountRepository accountRepository;
//
//    @Autowired
//    public AccountController(AccountRepository accountRepository) {
//        this.accountRepository = accountRepository;
//    }
//
//    @GetMapping("/username/{username}")
//    public BaseResponse<Account> getAccount(@PathVariable String username) {
//        return new BaseResponse<Account>(true, "Account found", accountRepository.findByUsername(username));
//    }
//
//    @GetMapping("/all")
//    public BaseResponse<List<Account>> getAllAccounts() {
//        return new BaseResponse<List<Account>>(true, "Accounts retrieved", accountRepository.findAll());
//    }
//
//    @PostMapping("/create")
//    public BaseResponse<Account> createAccount(@RequestParam String username, @RequestParam String password) {
//        if(accountRepository.findByUsername(username) != null) {
//            return new BaseResponse<Account>(false, "Account already exists", null);
//        }
//        Account account = accountRepository.save(new Account(username, password));
//        return new BaseResponse<Account>(true, "Account created", account);
//    }
//}
