using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Entities;
using Domain.Abstract;
using Neo4jCinema.Models;
using Neo4jClient;

namespace Neo4jCinema.Controllers
{
    public class MovieController : SecurityController
    {
        private IMovieRepository movieRepo;
        private IActorRepository actorRepo;
        private IUserRepository userRepo;
        private IGraphClient graphClient;

        public MovieController(IMovieRepository moRepo, IActorRepository acRepo, IUserRepository usRepo, IGraphClient client) : base(usRepo, client)
        {
            movieRepo = moRepo;
            actorRepo = acRepo;
            userRepo = usRepo;
            graphClient = client;
        }

        // GET: Movie
        public ActionResult Index()
        {
            MovieListViewModel vm = new MovieListViewModel();
            vm.Movies = movieRepo.GetMovies();

            return View(vm);
        }

        public ViewResult MovieDetails(string userId, string movieId)
        {
            MovieDetailsViewModel vm = new MovieDetailsViewModel();
            vm.CurrentUser = userRepo.GetUserById(userId);
            vm.Movie = movieRepo.GetMovieById(movieId);
            vm.MovieActors = vm.Movie.ReturnAllActorsFromMovie(movieId, graphClient);

            if(vm.Movie.MovieWatchedByUser(movieId, userId, graphClient) == true)
            {
                ViewBag.MovieWatched = 1;
            }
            else
            {
                ViewBag.MovieWatched = 0;
            }

            return View("MovieDetails", vm);
        }


        public ActionResult WatchedMovie(string userId, string movieId)
        {
            Movie movie = movieRepo.GetMovieById(movieId);
            movie.RemoveHaventWatchedRelationship(movieId, userId, graphClient);
            movie.WatchMovie(movieId, userId, graphClient);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult HaventWatchedMovie(string userId, string movieId)
        {
            Movie movie = movieRepo.GetMovieById(movieId);
            movie.HaventWatchedMovie(movieId, userId, graphClient);

            return Redirect(Request.UrlReferrer.ToString());

        }

        public ViewResult EditMovie(string movieId)
        {
            EditMovieViewModel vm = new EditMovieViewModel();

            vm.Movie = movieRepo.GetMovieById(movieId);
            vm.MovieActors = vm.Movie.ReturnAllActorsFromMovie(movieId, graphClient);
            vm.ExistingActors = actorRepo.GetActors();
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditMovie(Movie movie)
        {
            if (movie.MovieId == null)
            {
                // Create
                movieRepo.InsertMovie(movie);
            }

            else
            {
                // Update
                movieRepo.UpdateMovie(movie);
            }

            return RedirectToAction("Index");

        }

        public ViewResult CreateMovie()
        {
            EditMovieViewModel vm = new EditMovieViewModel();
            vm.Movie = new Movie();
            vm.MovieActors = Enumerable.Empty<Actor>();
            vm.ExistingActors = actorRepo.GetActors();
            return View("EditMovie", vm);
        }

        [HttpPost]
        public ActionResult DeleteMovie(string movieId)
        {
            if(movieId != null)
            {
                movieRepo.DeleteMovie(movieId);
            }

            return RedirectToAction("Index");
        }

        
        public ActionResult RemoveActingRelationship(string movieId, string actorId)
        {
            Movie movie = movieRepo.GetMovieById(movieId);
            movie.RemoveActorFromMovie(movieId, actorId, graphClient);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult AddActingRelationship(string movieId, string actorId)
        {
            Movie movie = movieRepo.GetMovieById(movieId);
            movie.AddActorToMovie(movieId, actorId, graphClient);

            return Redirect(Request.UrlReferrer.ToString()); 
        }
    }
}