using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Neo4jCinema.Models
{
    public class ActorListViewModel
    {
        public IEnumerable<Actor> Actors { get; set; }
    }
}