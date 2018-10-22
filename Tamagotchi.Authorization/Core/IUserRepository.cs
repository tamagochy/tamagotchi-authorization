using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization.Core
{
    public interface IUserRepository
    {
        User GetUserByLogin(string login);
        User GetUserByEmail(string eMail);
        void AddUser(User user);
        void UpdatePassword(User user, string newPassword);
    }
}
