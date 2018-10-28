
namespace Tamagotchi.Authorization.Helpers
{
    public class Hashing
    {
        public static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public static bool ValidatePassword(string password, string correctHash) =>
            BCrypt.Net.BCrypt.Verify(password, correctHash);
    } 
}
