using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstract
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        IEnumerable<User> GetUsersThatContainUsername(string username);
        User GetUserById(string userId);
        User GetUserByUsername(string username);
        void InsertUser(User user);
        void DeleteUser(string userId);
        void UpdateUser(User user);
    }
}
