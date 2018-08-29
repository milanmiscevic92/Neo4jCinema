using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Neo4jCinema.Models
{
    public class MovieListViewModel
    {
        public IEnumerable<Movie> Movies { get; set; }
    }
}