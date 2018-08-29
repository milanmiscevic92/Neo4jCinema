using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Event
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public string EventDescription { get; set; }

        
        public DateTime EventStartTime { get; set; }

        public void AddEventLocation(string eventId, string locationId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(e:Event)", "(c:Cinema)")
                .Where((Event e) => e.EventId == eventId)
                .AndWhere((Cinema c) => c.CinemaId == locationId)
                .Create("(e)-[:IS_SHOWING_IN]->(c)")
                .ExecuteWithoutResults();
        }

        public void RemoveEventLocation(string eventId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(e:Event)-[r:IS_SHOWING_IN]->(c)")
                .Where((Event e) => e.EventId == eventId)
                .Delete("r")
                .ExecuteWithoutResults();
        }

        public string ReturnEventLocationName(string eventId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            string cinemaName;

            cinemaName = graphClient.Cypher
                .OptionalMatch("(e:Event)-[r:IS_SHOWING_IN]->(c:Cinema)")
                .Where((Event e) => e.EventId == eventId)
                .Return<string>("c.CinemaName")
                .Results.Single();

            if (cinemaName == null)
            {
                return "None";
            }
            else
            {
                return cinemaName;
            }
        }

        public string ReturnEventLocationId(string eventId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            string cinemaId;

            cinemaId = _graphClient.Cypher
                .OptionalMatch("(e:Event)-[r:IS_SHOWING_IN]->(c:Cinema)")
                .Where((Event e) => e.EventId == eventId)
                .Return<string>("c.CinemaId")
                .Results.Single();

            if (cinemaId == null)
            {
                return "None";
            }
            else
            {
                return cinemaId;
            }
        }

        public void AttendEvent(string userId, string eventId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(u:User)", "(e:Event)")
                .Where((User u) => u.UserId == userId)
                .AndWhere((Event e) => e.EventId == EventId)
                .CreateUnique("(u)-[:IS_ATTENDING]->(e)")
                .ExecuteWithoutResults();
        }

        public void CancelEvent(string userId, string eventId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .OptionalMatch("(u:User)-[r:IS_ATTENDING]->(e:Event)")
                .Where((User u) => u.UserId == userId)
                .AndWhere((Event e) => e.EventId == eventId)
                .Delete("r")
                .ExecuteWithoutResults();
        }

        public bool EventAttendedByUser(string userId, string eventId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;
            User tempUser;

            tempUser = _graphClient.Cypher
                .OptionalMatch("(u:User)-[r:IS_ATTENDING]->(e:Event)")
                .Where((User u) => u.UserId == userId)
                .AndWhere((Event e) => e.EventId == eventId)
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

        // Not tested!
        public int NumberOfFriendsAttending(string userId, string eventId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            IEnumerable<User> tempUsers = _graphClient.Cypher
               .OptionalMatch("(u1:User)-[:IS_FOLLOWING]-(u2:User)-[:IS_ATTENDING]->(e:Event)")
               .Where((User u1) => u1.UserId == userId)
               .AndWhere((Event e) => e.EventId == eventId)
               .ReturnDistinct(u2 => u2.As<User>())
               .Results.ToList<User>();

            int numberOfFriendsAttending = tempUsers.Count();

            return numberOfFriendsAttending;
        }

        public IEnumerable<User> ReturnAllFriendsAttending(string userId, string eventId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            return _graphClient.Cypher
               .OptionalMatch("(u1:User)-[:IS_FOLLOWING]-(u2:User)-[:IS_ATTENDING]->(e:Event)")
               .Where((User u1) => u1.UserId == userId)
               .AndWhere((Event e) => e.EventId == eventId)
               .ReturnDistinct(u2 => u2.As<User>())
               .Results.ToList<User>();
        }
    }
}
