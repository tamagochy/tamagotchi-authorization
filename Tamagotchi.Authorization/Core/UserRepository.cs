using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tamagotchi.Authorization.Helpers;
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
        public async Task<User> GetUserById(int id) =>
            await _db.TamagotchiUser.FirstOrDefaultAsync(user => user.UserId.Equals(id));
        public async Task<User> GetUserByLogin(string login) =>
             await _db.TamagotchiUser.FirstOrDefaultAsync(user => user.Login.Equals(login));

        public async Task<User> GetUserByEmail(string eMail) =>
            await _db.TamagotchiUser.FirstOrDefaultAsync(user => user.Email.Equals(eMail));
        public async Task AddUser(User user, int countRound)
        {
            user.Password = Hashing.HashPassword(user.Password, countRound);
            _db.TamagotchiUser.Add(user);
            await _db.SaveChangesAsync();
        }
        public async Task UpdatePassword(User user, string newPassword, int countRound)
        {
            _db.Entry(user).State = EntityState.Modified;
            user.Password = Hashing.HashPassword(newPassword, countRound);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByIds(int[] ids)
        {
            var result = new List<User>();
            foreach(var id in ids)
            {
                var user = await _db.TamagotchiUser.FirstOrDefaultAsync(x => x.UserId == id);
                if (user != null)
                    result.Add(user);
            }
            return result;
        }
    }
}
