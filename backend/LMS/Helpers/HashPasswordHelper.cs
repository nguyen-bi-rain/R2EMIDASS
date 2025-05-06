using System.Security.Cryptography;

namespace LMS.Helpers;

public static class HashPasswordHelper
{
    public static string HashPassword(string password)
    {
        // Generate a 128-bit salt (16 bytes)
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // Use PBKDF2 with SHA256, 100,000 iterations
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32); // 256-bit hash

        // Combine iterations, salt, and hash into one string
        return $"100000.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    // Verify password against a stored hash
    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 3)
            return false;

        int iterations = int.Parse(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] storedHashBytes = Convert.FromBase64String(parts[2]);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(hash, storedHashBytes);
    }
}

public interface IHashPasswordHelper
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string storedHash);
}

public class HashPasswordHelperWrapper : IHashPasswordHelper
{
    public string HashPassword(string password)
    {
        return HashPasswordHelper.HashPassword(password);
    }
    public bool VerifyPassword(string password, string storedHash)
    {
        return HashPasswordHelper.VerifyPassword(password, storedHash);
    }
}