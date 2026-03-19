using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ContactManagerAPI.Context;
using ContactManagerAPI.Model;
using ContactManagerAPI.Model.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ContactManagerAPI.Services
{
    public class AccountService
    {
        private readonly DataContext _userData;
        private readonly IConfiguration _config;

        public AccountService(DataContext userData, IConfiguration config)
        {
            _userData = userData;
            _config = config;
        }

        public async Task<bool> CreateAccount(CreateAccountDTO newUser)
        {
            if(await DoesNameExistAsync(newUser.Username)) return false;
            if(await DoesEmailExistAsync(newUser.Email)) return false;

            UserInfoModel createUser = new();
            //create function to HashPassword and return encryptedPassword
            var encryptedPassword = HashPassword(newUser.Password);
            createUser.Username = newUser.Username;
            createUser.Email = newUser.Email;
            createUser.Salt = encryptedPassword.Salt;
            createUser.Hash = encryptedPassword.Hash;
            
            await _userData.Users.AddAsync(createUser);
            return await _userData.SaveChangesAsync() != 0;
        }

        // ? Helper Functions to check for DoesUsernameExist and DoesEmailExistAsync and HashPassword
        private async Task<bool> DoesNameExistAsync(string username) => await _userData.Users.SingleOrDefaultAsync(user => user.Username == username) != null;
        
        private async Task<bool> DoesEmailExistAsync(string email) => await _userData.Users.SingleOrDefaultAsync(user => user.Email == email) != null;

        private static PasswordDTO HashPassword(string password)
        {
            byte[] SaltBytes = RandomNumberGenerator.GetBytes(64);

            string salt = Convert.ToBase64String(SaltBytes);
            string hash;
            
            using (var derivedBytes = new Rfc2898DeriveBytes(password, SaltBytes, 310000, HashAlgorithmName.SHA256))
            {
                hash = Convert.ToBase64String(derivedBytes.GetBytes(32));
            }
            return new PasswordDTO
            {
                Salt = salt,
                Hash = hash
            };
        }
        
        // ? End of helper functions for create account!


        public async Task<string> Login(LoginDTO userLogin)
        {
            UserInfoModel currentUser = await GetUserInfoByUsernameAsync(userLogin.LoginParam);
            if(currentUser == null) currentUser = await GetUserinfoByEmailAsync(userLogin.LoginParam);

            if(currentUser == null) return null;

            //will have funtion to verify password
            if(!VerifyPassword(userLogin.Password, currentUser.Salt, currentUser.Hash)) return null;

            return GenerateJWT(new List<Claim>());
            
        }

        // ? Helper functions to login
        private async Task<UserInfoModel> GetUserInfoByUsernameAsync(string username) => await _userData.Users.SingleOrDefaultAsync(user => user.Username == username);
        private async Task<UserInfoModel?> GetUserinfoByEmailAsync(string email) => await _userData.Users.SingleOrDefaultAsync(user => user.Email == email);
        private static bool VerifyPassword(string password, string salt, string hash)
        {
            byte[] SaltBytes = Convert.FromBase64String(salt);
            string checkHash;
            
            using (var derivedBytes = new Rfc2898DeriveBytes(password, SaltBytes, 310000, HashAlgorithmName.SHA256))
            {
                checkHash = Convert.ToBase64String(derivedBytes.GetBytes(32));
                return hash == checkHash;
            }
        }
        private string GenerateJWT(List<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));

            var SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
// https://contactmanagerdbh.vercel.app
            var tokenOptions = new JwtSecurityToken(
                issuer: "https://contactmanagerdbhapi-cnfhhvf7e7fse7gt.westus3-01.azurewebsites.net/",
                audience: "https://contactmanagerdbhapi-cnfhhvf7e7fse7gt.westus3-01.azurewebsites.net/",
                claims: claims,
                expires: DateTime.Now.AddMinutes(45),
                signingCredentials: SigningCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public async Task<bool> EditUsername(int id, CreateAccountDTO updateUser)
        {
            var currentUser = await _userData.Users.FindAsync(id);

            if(currentUser == null) return false;

            var securePassword = HashPassword(updateUser.Password);

            currentUser.Username = updateUser.Username;
            currentUser.Email = updateUser.Password;
            currentUser.Hash = securePassword.Hash;
            currentUser.Salt = securePassword.Salt;
            _userData.Users.Update(currentUser);
            return await _userData.SaveChangesAsync() != null;
        }
    }
}