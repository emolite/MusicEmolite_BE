using System;
using System.Security.Cryptography;
using System.Text;

namespace MS_Application.Helpers
{
    public static class HashHelper
    {
        private const int SaltSize = 64;

        public static void CreatePasswordHash(string password, out string passwordHashBase64, out string passwordSaltBase64)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password không được rỗng hoặc null");

            var saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            using (var hmac = new HMACSHA512(saltBytes))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                passwordHashBase64 = Convert.ToBase64String(hashBytes);
                passwordSaltBase64 = Convert.ToBase64String(saltBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHashBase64) || string.IsNullOrWhiteSpace(storedSaltBase64))
                return false;

            var saltBytes = Convert.FromBase64String(storedSaltBase64);
            var storedHashBytes = Convert.FromBase64String(storedHashBase64);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                if (computedHash.Length != storedHashBytes.Length) return false;
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHashBytes[i]) return false;
                }
            }

            return true;
        }
    }
}