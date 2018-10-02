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
            //TODO: жду БД
            TestUsers = new List<User>
            {
                new User { Login = "testov", Password = "12345", Email = "test@test.te", Pet = 1 },
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
    }
}
