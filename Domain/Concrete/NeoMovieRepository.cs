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
    public class NeoMovieRepository : IMovieRepository
    {
        private readonly IGraphClient _graphClient;

        public NeoMovieRepository(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public IEnumerable<Movie> GetMovies()
        {
            return _graphClient.Cypher
                .Match("(m:Movie)")
                .Return(m => m.As<Movie>())
                .Results.ToList<Movie>();
        }

        public IEnumerable<Movie> GetMoviesThatContainString(string searchString)
        {
            return _graphClient.Cypher
               .OptionalMatch("(m:Movie)")
               .Where("(m.MovieName =~ {searchString})")
               .WithParam("searchString", "(?i).*" + searchString + ".*")
               .Return(m => m.As<Movie>())
               .Results.ToList<Movie>();
        }


        public Movie GetMovieById(string movieId)
        {
            return _graphClient.Cypher
                .Match(" (m:Movie {MovieId:{movieId}} ) ")
                .WithParam("movieId", movieId)
                .Return(m => m.As<Movie>())
                .Results.Single();
        }

        public Dictionary<string, int> GetTopMovies()
        {
            Dictionary<string, int> topMovies = new Dictionary<string, int>();

            int numberOfWatchedRelationships;

            IEnumerable<Movie> allMovies = _graphClient.Cypher
                .OptionalMatch("(m:Movie)<-[r:HAS_WATCHED_MOVIE]-(u:User)")
                .ReturnDistinct(m => m.As<Movie>()).Results.ToList();

            foreach (Movie mo in allMovies)
            {
                if (mo != null)
                {
                    IEnumerable<User> usersThatWatched = _graphClient.Cypher
                        .OptionalMatch("(m:Movie)<-[r:HAS_WATCHED_MOVIE]-(u:User)")
                        .Where((Movie m) => m.MovieId == mo.MovieId)
                        .ReturnDistinct(u => u.As<User>())
                        .Results.ToList();

                    numberOfWatchedRelationships = usersThatWatched.Count();

                    topMovies.Add(mo.MovieId, numberOfWatchedRelationships);
                }
            }



            return topMovies;

        }

        public void InsertMovie(Movie movie)
        {
            movie.MovieId = Guid.NewGuid().ToString();

            Movie mo = _graphClient.Cypher
                .Create(" (m:Movie {movie}) ")
                .WithParam("movie", movie)
                .Return(m => m.As<Movie>())
                .Results.Single();
        }

        public void UpdateMovie(Movie movie)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("name", movie.MovieName);
            queryDict.Add("genre", movie.MovieGenre);
            queryDict.Add("director", movie.MovieDirector);
            queryDict.Add("year", movie.ReleaseYear);

            _graphClient.Cypher
                .Match("(m:Movie)")
                .Where((Movie m) => m.MovieId == movie.MovieId)
                .Set("m.MovieName = {name}, m.MovieGenre = {genre}, m.MovieDirector = {director}, m.ReleaseYear = {year}")
                .WithParams(queryDict)
                .ExecuteWithoutResults();
        }

        public void DeleteMovie(string movieId)
        {
            _graphClient.Cypher
                .Match(" (m:Movie {MovieId:{movieId}} ) ")
                .WithParam("movieId", movieId)
                .DetachDelete("m")
                .ExecuteWithoutResults();
        }
    }
}
