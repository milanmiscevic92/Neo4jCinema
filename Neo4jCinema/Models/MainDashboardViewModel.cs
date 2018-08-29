using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;
using System.Web.Mvc;

namespace Neo4jCinema.Models
{
    public class MainDashboardViewModel : BaseViewModel
    {
        public string SearchString { get; set; }
        public string SelectedSearchCategory { get; set; }
        public List<SelectListItem> CategoriesSelectList { get; set; }

        public IEnumerable<Movie> FoundMovies { get; set; }
        public IEnumerable<Event> FoundEvents { get; set; }
        public IEnumerable<Actor> FoundActors { get; set; }

        public List<KeyValuePair<Event, int>> TopEvents { get; set; }
        public List<KeyValuePair<Movie, int>> TopMovies { get; set; }
    }
}