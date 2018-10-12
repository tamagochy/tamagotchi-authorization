using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using tamagotchi_authorization.Models;

namespace tamagotchi_authorization.Core
{
    internal class UserRepository
    {
        private UserContext _db;
        public UserRepository(UserContext context)
        {
            _db = context;
        }
        public User GetUser(string login)
        {
            try
            {
                return _db.User.First(user => user.Login.Equals(login));
            }
            catch
            {
                return null;
            }
        }
        public User GetUserByEMail(string email) =>
            _db.User.FirstOrDefault(user => user.Email.Equals(email));

        public void AddUser(string login, string password, string email, string pet) =>
            _db.User.Add(new User
            {
                Login = login,
                Password = password,
                Email = email,
                Pet = int.Parse(pet) 
            });

        public void UpdatePassword(User user, string password)
        {
            _db.Entry(user).State = EntityState.Modified;
            user.Password = password;
            _db.SaveChanges();
        }


    }
}
