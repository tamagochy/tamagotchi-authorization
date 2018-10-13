
using tamagotchi_authorization.Models;

namespace tamagotchi_authorization.Core
{
    internal interface IUserRepository
    {
        User GetUserByLogin(string login);
        User GetUserByEmail(string eMail);
        void AddUser(User user);
        void UpdatePassword(User user, string newPassword);
    }
}
