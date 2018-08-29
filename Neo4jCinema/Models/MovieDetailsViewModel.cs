using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Neo4jCinema.Models
{
    public class MovieDetailsViewModel : BaseViewModel
    {
        public Movie Movie { get; set; }
        public IEnumerable<Actor> MovieActors { get; set; }
    }
}