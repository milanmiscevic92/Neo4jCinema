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
    public class MainDashboardController : SecurityController
    {
        private IUserRepository userRepo;
        private IMovieRepository movieRepo;
        private IEventRepository eventRepo;
        private IActorRepository actorRepo;
        private IGraphClient graphClient;

        public MainDashboardController(IUserRepository usRepo, IMovieRepository moRepo, IEventRepository evRepo, IActorRepository acRepo, IGraphClient client):base(usRepo,client)
        {
            userRepo = usRepo;
            movieRepo = moRepo;
            eventRepo = evRepo;
            actorRepo = acRepo;
            graphClient = client;
        }

        // GET: MainDashboard
        public ActionResult Index()
        {
            MainDashboardViewModel vm = new MainDashboardViewModel();

            vm.CurrentUser = userRepo.GetUserByUsername(Request.Cookies[CinemaUserAuthKey].Value);

            vm.FoundActors = Enumerable.Empty<Actor>();
            vm.FoundMovies = Enumerable.Empty<Movie>();
            vm.FoundEvents = Enumerable.Empty<Event>();
            vm.TopMovies = new List<KeyValuePair<Movie, int>>();
            vm.TopEvents = new List<KeyValuePair<Event, int>>();

            Dictionary<string, int> topMoviesIdName = new Dictionary<string, int>();
            topMoviesIdName = movieRepo.GetTopMovies();

            Dictionary<Movie, int> topMoviesMovieWatches = new Dictionary<Movie, int>();
            Dictionary<Event, int> topEventAttending = new Dictionary<Event, int>();
            
            foreach(var m in topMoviesIdName)
            {
                topMoviesMovieWatches.Add(movieRepo.GetMovieById(m.Key), m.Value);
            }

            List<KeyValuePair<Movie, int>> sortedMovies = (from kv in topMoviesMovieWatches orderby kv.Value descending select kv).ToList();
            vm.TopMovies = sortedMovies;


            foreach(var e in eventRepo.GetTopEvents())
            {
                topEventAttending.Add(e.Key, e.Value);
            }

            List<KeyValuePair<Event, int>> sortedEvents = (from kv in topEventAttending orderby kv.Value descending select kv).ToList();
            vm.TopEvents = sortedEvents;

            vm.CategoriesSelectList = new List<SelectListItem>();
            vm.CategoriesSelectList.Add(new SelectListItem { Text = "Movies", Value = "0" });
            vm.CategoriesSelectList.Add(new SelectListItem { Text = "Actors", Value = "1" });
            vm.CategoriesSelectList.Add(new SelectListItem { Text = "Events", Value = "2" });

            return View(vm);
        }

        public ActionResult SearchByCategory(string userId, MainDashboardViewModel vm)
        {
            vm.CurrentUser = userRepo.GetUserById(userId);
            vm.TopMovies = new List<KeyValuePair<Movie, int>>();
            vm.TopEvents = new List<KeyValuePair<Event, int>>();

            Dictionary<string, int> topMoviesIdName = new Dictionary<string, int>();
            topMoviesIdName = movieRepo.GetTopMovies();

            Dictionary<Movie, int> topMoviesMovieWatches = new Dictionary<Movie, int>();
            Dictionary<Event, int> topEventAttending = new Dictionary<Event, int>();

            foreach (var m in topMoviesIdName)
            {
                topMoviesMovieWatches.Add(movieRepo.GetMovieById(m.Key), m.Value);
            }

            List<KeyValuePair<Movie, int>> sorted = (from kv in topMoviesMovieWatches orderby kv.Value descending select kv).ToList();
            vm.TopMovies = sorted;

            foreach (var e in eventRepo.GetTopEvents())
            {
                topEventAttending.Add(e.Key, e.Value);
            }

            List<KeyValuePair<Event, int>> sortedEvents = (from kv in topEventAttending orderby kv.Value descending select kv).ToList();
            vm.TopEvents = sortedEvents;


            vm.CategoriesSelectList = new List<SelectListItem>();
            vm.CategoriesSelectList.Add(new SelectListItem { Text = "Movies", Value = "0" });
            vm.CategoriesSelectList.Add(new SelectListItem { Text = "Actors", Value = "1" });
            vm.CategoriesSelectList.Add(new SelectListItem { Text = "Events", Value = "2" });

            if (vm.SelectedSearchCategory == "0")
            {
                vm.FoundMovies = movieRepo.GetMoviesThatContainString(vm.SearchString);

                ViewBag.Category = "0";
            }

            else
                if(vm.SelectedSearchCategory == "1")
            {
                vm.FoundActors = actorRepo.GetActorsThatContainString(vm.SearchString);

                ViewBag.Category = "1";
            }
            else
                if(vm.SelectedSearchCategory == "2")
            {
                vm.FoundEvents = eventRepo.GetEventsThatContainString(vm.SearchString);

                ViewBag.Category = "2";
            }

            return View("Index", vm);

        }
    }
}