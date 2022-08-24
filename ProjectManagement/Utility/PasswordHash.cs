using System.Security.Cryptography;
using System.Text;

namespace ProjectManagement.Utility
{
    public class PasswordHash
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var passwordComputed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return passwordComputed.SequenceEqual(passwordHash);
        }
    }
}
