using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Neo4jCinema.Models
{
    public class ActorDetailsViewModel : BaseViewModel
    {
        public Actor Actor { get; set; }
        public IEnumerable<Movie> ActorsMovies { get; set; }
    }
}