using BCrypt;

namespace Tamagotchi.Authorization.Helpers
{
    public class Hashing
    {
        public static string HashPassword(string password, int countRound)
        {
            var salt = BCryptHelper.GenerateSalt(countRound);
            return BCryptHelper.HashPassword(password, salt);
        }
        public static bool ValidatePassword(string password, string correctHash)
        {
            return BCryptHelper.CheckPassword(password, correctHash);
        }
    } 
}
