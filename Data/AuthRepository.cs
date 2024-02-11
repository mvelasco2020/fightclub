using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fightclub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace fightclub.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            this._context = context;

        }

        public Task<ServiceResponse<string>> Login(string username, string password)
        {
            throw new NotImplementedException();
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

    }
}