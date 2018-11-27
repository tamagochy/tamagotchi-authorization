using System.Collections.Generic;
using System.Threading.Tasks;
using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization.Core
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByLogin(string login);
        Task<User> GetUserByEmail(string eMail);
        Task<bool> CheckExistsUser(string login, string mail);
        Task AddUser(User user, int countRound);
        Task UpdatePassword(User user, string newPassword, int countRound);
        Task<IEnumerable<User>> GetUsersByIds(int[] ids);
    }
}
