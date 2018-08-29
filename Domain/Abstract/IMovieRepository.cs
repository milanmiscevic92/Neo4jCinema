using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstract
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetMovies();
        IEnumerable<Movie> GetMoviesThatContainString(string searchString);
        Dictionary<string, int> GetTopMovies();
        Movie GetMovieById(string movieId);
        void InsertMovie(Movie movie);
        void DeleteMovie(string movieId);
        void UpdateMovie(Movie movie);
    }
}
