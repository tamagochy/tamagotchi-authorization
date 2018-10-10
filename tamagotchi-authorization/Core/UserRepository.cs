using System.Collections.Generic;
using System.Linq;
using tamagotchi_authorization.Models;

namespace tamagotchi_authorization.Core
{
    public class UserRepository
    {
        public List<User> TestUsers;
        public UserRepository()
        {
            TestUsers = new List<User>
            {
                new User { UserId = 1, Login = "testov", Password = "12345", Email = "test@test.te", Pet = 3 },
            };            
        }
        public User GetUser(string login)
        {
            try
            {
                return TestUsers.First(user => user.Login.Equals(login));
            }
            catch
            {
                return null;
            }
        }
        public User GetUserByEMail(string email) =>
            TestUsers.FirstOrDefault(user => user.Email.Equals(email));
    }
}
