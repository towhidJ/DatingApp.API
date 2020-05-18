﻿using System.Threading.Tasks;
using DatingApp.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Data
{
    public class AuthRepository:IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AppSettings _appSettings;

        public AuthRepository(ApplicationDbContext db,IOptions<AppSettings> appSettings)
        {
            _db = db;
            _appSettings = appSettings.Value;
        }
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user==null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;
            return user;

        }



        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i]!=passwordHash[i])
                    {
                        return false;
                    }

                    
                }
                
            }
            return true;
        }

        //public IEnumerable<User> GetAll()
        //{
        //    return _users.WithoutPasswords();
        //}

        public async Task<bool> UserExists(string username)
        {
            if (await _db.Users.AnyAsync(x => x.Username == username))
                return true;
            return false;
        }
    }
}
