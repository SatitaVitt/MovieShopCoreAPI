using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MovieShop.Infrastructure.Services
{
    public class CryptoService : ICryptoService
    {
        //when working with Hashing, never create your own hashing algorithm
        //always use industry hashing algorithm that have been used and popular:
        //Argon2id, PBKDF2, Bcrypt
        //Microsoft has already implemented PBKDF2 Algorithm
        public string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public string HashPassword(string password, string salt)
        {
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                                     password: password,
                                                                     salt: Convert.FromBase64String(salt),
                                                                     prf: KeyDerivationPrf.HMACSHA512,
                                                                     iterationCount: 10000,
                                                                     numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
