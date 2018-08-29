using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;
using System.Web.Mvc;

namespace Neo4jCinema.Models
{
    public class EditMovieViewModel
    {
        public Movie Movie { get; set; }
        public IEnumerable<Actor> MovieActors { get; set; }
        public IEnumerable<Actor> ExistingActors { get; set; }
    }
}