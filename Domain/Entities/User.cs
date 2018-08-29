using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Domain.Entities
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool AuthenticatePassword(string password)
        {
            if(this.Password == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool EmailBelongsToAnotherUser(string email, string myId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;
            User tempUser;

            tempUser = _graphClient.Cypher
                .OptionalMatch("(u:User)")
                .Where((User u) => u.EmailAddress == email)
                .Return(u => u.As<User>())
                .Results.Single();

            if(tempUser == null)
            {
                return false;
            }

            else if(tempUser.UserId == myId)
            {
                return false;
            }
            else
            {
                return true;
            }


        }
        public bool EmailAddressExists(string email, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;
            User tempUser;

            tempUser = _graphClient.Cypher
                .OptionalMatch("(u:User)")
                .Where((User u) => u.EmailAddress == email)
                .Return(u => u.As<User>())
                .Results.Single();

            if(tempUser == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        public bool UsernameExists(string username, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;
            User tempUser;

            tempUser = _graphClient.Cypher
                .OptionalMatch("(u:User)")
                .Where((User u) => u.Username == username)
                .Return(u => u.As<User>())
                .Results.Single();


            if(tempUser == null)
            {
                return false;
            }

            else
            {
                return true;
            }
        }


        public void FollowUser(string followsId, string followedUsername, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(u1:User)", "(u2:User)")
                .Where((User u1) => u1.UserId == followsId)
                .AndWhere((User u2) => u2.Username == followedUsername)
                .CreateUnique("(u1)-[:IS_FOLLOWING]->(u2)")
                .ExecuteWithoutResults();
        }

        public void UnfollowUser(string followsId, string unfollowedUsername, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(u1:User)-[r:IS_FOLLOWING]->(u2:User)")
                .Where((User u1) => u1.UserId == followsId)
                .AndWhere((User u2) => u2.Username == unfollowedUsername)
                .Delete("r")
                .ExecuteWithoutResults();
        }



        public IEnumerable<User> ReturnFollowing(string userId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            return _graphClient.Cypher
                .Match("(u1:User)-[r:IS_FOLLOWING]->(u2:User)")
                .Where((User u1) => u1.UserId == UserId)
                .Return(u2 => u2.As<User>())
                .Results.ToList<User>();
        }

        public IEnumerable<User> ReturnFollowers(string userId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            return _graphClient.Cypher
                .Match("(u1:User)<-[r:IS_FOLLOWING]-(u2:User)")
                .Where((User u1) => u1.UserId == UserId)
                .Return(u2 => u2.As<User>())
                .Results.ToList<User>();
        }
    }
}
