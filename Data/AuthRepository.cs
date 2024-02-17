using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using fightclub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace fightclub.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            if (await UserExists(username))
            {
                var user = await _context.Users.FirstAsync(u => u.Username.ToLower() == username.ToLower());
                if (VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    response.Data = CreateToken(user);
                    return response;
                }

            }

            response.Success = false;
            response.Message = "Username and/or Passwords is incorrect";
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var response = new ServiceResponse<int>();
            if (await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = $"User {user.Username} already exists. please try again";
                return response;
            }

            CreateHashPassword(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id;

            return response;

        }

        public async Task<bool> UserExists(string username)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
            return userExists;
        }

        private void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return hash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
            };

            var appSettingsToken = _configuration.GetSection("AppSettings.Token").Value;
            if (appSettingsToken is null)
            {
                
            }
            return string.Empty;
        }
    }
}