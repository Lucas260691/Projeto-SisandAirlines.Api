using System.Security.Cryptography;
using System.Text;

namespace SisandAirlines.Application.Utils
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            using var sha = SHA512.Create();

            var bytes = Encoding.UTF8.GetBytes(password.Trim());
            var hashBytes = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool Verify(string password, string storedHash)
        {
            var computedHash = Hash(password);
            return computedHash == storedHash;
        }
    }
}
