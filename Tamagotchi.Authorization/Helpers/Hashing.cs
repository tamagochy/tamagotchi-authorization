
namespace Tamagotchi.Authorization.Helpers
{
    public class Hashing
    {
        public static string HashPassword(string password, int countRound) =>
            BCrypt.Net.BCrypt.HashPassword(password, countRound);

        public static bool ValidatePassword(string password, string correctHash) =>
            BCrypt.Net.BCrypt.Verify(password, correctHash);
    } 
}
