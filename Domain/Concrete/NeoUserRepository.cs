using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Entities;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Domain.Concrete
{
    public class NeoUserRepository : IUserRepository
    {
        private readonly IGraphClient _graphClient;

        public NeoUserRepository(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public IEnumerable<User> GetUsers()
        {
            return _graphClient.Cypher
                .Match("(u:User)")
                .Return(u => u.As<User>())
                .Results.ToList<User>();
        }

        public IEnumerable<User> GetUsersThatContainUsername(string username)
        {
            return _graphClient.Cypher
                .OptionalMatch("(u:User)")
                .Where("(u.Username =~ {username})")
                .WithParam("username", "(?i).*" +  username + ".*")
                .Return(u => u.As<User>())
                .Results.ToList<User>();
        }

        public User GetUserById(string userId)
        {
            return _graphClient.Cypher
                .Match("(u:User {UserId:{userId}})")
                .WithParam("userId", userId)
                .Return(u => u.As<User>())
                .Results.Single();
        }

        public User GetUserByUsername(string username)
        {
            return _graphClient.Cypher
                .OptionalMatch("(u:User {Username:{username}})")
                .WithParam("username", username)
                .Return(u => u.As<User>())
                .Results.Single();
        }

        public void InsertUser(User user)
        {
            user.UserId = Guid.NewGuid().ToString();

            User us = _graphClient.Cypher
                .Create("(u:User {user})")
                .WithParam("user", user)
                .Return(u => u.As<User>())
                .Results.Single();
        }

        public void UpdateUser(User user)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("fname", user.FirstName);
            queryDict.Add("lname", user.LastName);
            queryDict.Add("email", user.EmailAddress);
            queryDict.Add("newPassword", user.Password);

            _graphClient.Cypher
                .Match("(u:User)")
                .Where((User u) => u.UserId == user.UserId)
                .Set("u.FirstName = {fname}, u.LastName = {lname}, u.EmailAddress = {email}, u.Password = {newPassword}")
                .WithParams(queryDict)
                .ExecuteWithoutResults();
        }

        public void DeleteUser(string userId)
        {
            _graphClient.Cypher
                .Match("(u:User {UserId:{userId}})")
                .WithParam("userId", userId)
                .DetachDelete("u")
                .ExecuteWithoutResults();
        }


    }
}
