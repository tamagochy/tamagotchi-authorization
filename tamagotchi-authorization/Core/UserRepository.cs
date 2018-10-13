using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using tamagotchi_authorization.Models;

namespace tamagotchi_authorization.Core
{
    internal class UserRepository : IUserRepository
    {
        private readonly UserContext _db;
        private readonly ILogger _logger;

        public UserRepository(UserContext context, ILoggerFactory loggerFactory)
        {
            _db = context;
            _logger = loggerFactory.CreateLogger("UserRepository");
        }

        public User GetUserByLogin(string login) =>
            _db.TamagotchiUser.First(user => user.Login.Equals(login));

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
