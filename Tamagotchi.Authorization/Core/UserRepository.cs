using Microsoft.EntityFrameworkCore;
using System.Linq;
using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization.Core
{
    internal class UserRepository : IUserRepository
    {
        private readonly UserContext _db;
        public UserRepository(UserContext context)
        {
            _db = context;
        }
        public User GetUserByLogin(string login) =>
            _db.TamagotchiUser.FirstOrDefault(user => user.Login.Equals(login));

        public User GetUserByEmail(string eMail) =>
            _db.TamagotchiUser.FirstOrDefault(user => user.Email.Equals(eMail));
        public void AddUser(User user)
        {
            _db.TamagotchiUser.Add(user);
            _db.SaveChanges();
        }
        public void UpdatePassword(User user, string newPassword)
        {
            _db.Entry(user).State = EntityState.Modified;
            user.Password = newPassword;
            _db.SaveChanges();
        }
    }
}
