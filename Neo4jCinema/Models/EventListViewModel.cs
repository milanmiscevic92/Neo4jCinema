using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Neo4jCinema.Models
{
    public class EventListViewModel
    {
        public IEnumerable<Event> Events { get; set; }
    }
}