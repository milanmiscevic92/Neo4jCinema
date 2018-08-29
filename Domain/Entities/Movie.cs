using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Domain.Entities
{
    public class Movie
    {
        public string MovieId { get; set; }
        public string MovieName { get; set; }
        public string MovieGenre { get; set; }
        public string MovieDirector { get; set; }
        public int ReleaseYear { get; set; }

        public void RemoveHaventWatchedRelationship(string movieId, string userId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;
            _graphClient.Cypher
                .OptionalMatch("(u:User)-[r:HAVENT_WATCHED_MOVIE]->(m:Movie)")
                .Where((Movie m) => m.MovieId == movieId)
                .AndWhere((User u) => u.UserId == userId)
                .Delete("r")
                .ExecuteWithoutResults();
        }

        public bool MovieWatchedByUser(string movieId, string userId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;
            Movie tempMovie;

            tempMovie = _graphClient.Cypher
                .OptionalMatch("(u:User)-[r:HAS_WATCHED_MOVIE]->(m:Movie)")
                .Where((Movie m) => m.MovieId == movieId)
                .AndWhere((User u) => u.UserId == userId)
                .Return(m => m.As<Movie>())
                .Results.Single();

            if (tempMovie == null)
            {
                return false;
            }

            else
            {
                return true;
            }

        }

        public void WatchMovie(string movieId, string userId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(u:User)", "(m:Movie)")
                .Where((User u) => u.UserId == userId)
                .AndWhere((Movie m) => m.MovieId == movieId)
                .CreateUnique("(u)-[:HAS_WATCHED_MOVIE]->(m)")
                .ExecuteWithoutResults();
        }

        public void HaventWatchedMovie(string movieId, string userId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(u:User)", "(m:Movie)")
                .Where((User u) => u.UserId == userId)
                .AndWhere((Movie m) => m.MovieId == movieId)
                .CreateUnique("(u)-[:HAVENT_WATCHED_MOVIE]->(m)")
                .ExecuteWithoutResults();
        }

        // Not tested!
        public int NumberOfFriendsThatWatched(string userId, string movieId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            IEnumerable<User> tempUsers = _graphClient.Cypher
               .OptionalMatch("(u1:User)-[:IS_FOLLOWING]-(u2:User)-[:HAS_WATCHED_MOVIE]->(m:Movie)")
               .Where((User u1) => u1.UserId == userId)
               .AndWhere((Movie m) => m.MovieId == movieId)
               .ReturnDistinct(u2 => u2.As<User>())
               .Results.ToList<User>() ;

            int numberOfPeopleThatWatched = tempUsers.Count();

            return numberOfPeopleThatWatched;
        }

        public void AddActorToMovie(string movieId, string actorId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(m:Movie)", "(a:Actor)")
                .Where((Movie m) => m.MovieId == movieId)
                .AndWhere((Actor a) => a.ActorId == actorId)
                .CreateUnique("(a)-[:ACTS_IN]->(m)")
                .ExecuteWithoutResults();
        }

        public void RemoveActorFromMovie(string movieId, string actorId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(a:Actor)-[r:ACTS_IN]->(m:Movie)")
                .Where((Movie m) => m.MovieId == movieId)
                .AndWhere((Actor a) => a.ActorId == actorId)
                .Delete("r")
                .ExecuteWithoutResults();
        }

        public IEnumerable<Actor> ReturnAllActorsFromMovie(string movieId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;
            
             return _graphClient.Cypher
                .Match("(a:Actor)-[r:ACTS_IN]->(m:Movie)")
                .Where((Movie m) => m.MovieId == movieId)
                .Return(a => a.As<Actor>())
                .Results.ToList<Actor>();

        }

        public bool HasActors(string movieId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;
            IEnumerable<Actor> actorsList;
            
            actorsList = _graphClient.Cypher
                .OptionalMatch("(a:Actor)-[r:ACTS_IN]->(m:Movie)")
                .Where((Movie m) => m.MovieId == movieId)
                .Return(a => a.As<Actor>())
                .Results.ToList<Actor>();

            if(actorsList != null)
            {
                return false;
            }

            else
            {
                return true;
            }


        }
    }
}
