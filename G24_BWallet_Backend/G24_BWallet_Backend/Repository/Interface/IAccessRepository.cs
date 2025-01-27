﻿using G24_BWallet_Backend.Models;
using G24_BWallet_Backend.Models.ObjectType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G24_BWallet_Backend.Repository.Interface
{
    public interface IAccessRepository
    {
        Task<Account> GetAccountAsync(string phone, string password);
        Task<string> JWTGenerateAsync(string phone, int userId);
        Task<bool> CheckPhoneNumberExistAsync(string phone);
        Task RegisterNewUserAsync(string phone, string pass, string name,
          string fb, string bank);
        Task<bool> SendOtpTwilioAsync(string phone, string otp);
        Task<string> OTPGenerateAsync();
        Task<bool> CheckOTPAsync(string otp, string enter);
        Task SaveOTPAsync(string phone, string otp, string jwt);
        Task<string> EncryptAsync(string password);
        Task<string> DecryptAsync(string password);
        Task<int> ChangePassword(int userId, PasswordChangeParam p);
        Task<User> GetUserAsync(Account account);
        Task<List<User>> GetAllUserAsync();
        Task<bool> CheckPhoneFormat(string phone);
        Task UpdateUserProfile(User userEditInfo, int userId);
        Task<bool> CheckOTPTimeAsync(string phone, int minute);
        Task<int> NewPassword(NewPassword p);
    }
}
